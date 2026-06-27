using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pulse.App.Common.Services.Interfaces;
using Pulse.Domain.Common.Services;
using Pulse.Infra.Database.Contexts;
using Pulse.Infra.Database.Outbox.Queues;

namespace Pulse.Infra.Database.Outbox;

/*
 * Outbox processor with at-least-once delivery semantics.
 * 
 * - a message is not marked processed until it is sent to the queue
 * - if the app crashes after sending but before marking processed, the message may be sent again
 * - queue consumers should therefore be idempotent
 * - ProcessingOn prevents concurrent processors from taking the same row
 * - ClaimTimeout allows recovery if a processor crashes after claiming rows
 * - For PostgreSQL, FOR UPDATE SKIP LOCKED gives strong competing-consumer behavior.
 * - For SQLite, the atomic UPDATE ... RETURNING claim works because SQLite serializes writers.
 *      It will not scale as well as PostgreSQL, but it is safe for local/small deployments.
 */
public sealed class OutboxProcessor
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly DatabaseContext _context;
    private readonly DatabaseOptions _databaseOptions;
    private readonly ILogger<OutboxProcessor> _logger;
    private readonly IQueue _queue;

    private readonly uint _batchSize;
    private readonly TimeSpan _claimTimeout;

    public OutboxProcessor(
        IDateTimeProvider dateTimeProvider,
        DatabaseContext context,
        IOptions<DatabaseOptions> databaseOptions,
        IOptions<OutboxOptions> outboxOptions,
        ILogger<OutboxProcessor> logger,
        IQueue queue)
    {
        _dateTimeProvider = dateTimeProvider;
        _context = context;
        _databaseOptions = databaseOptions.Value;
        _logger = logger;
        _queue = queue;

        _batchSize = outboxOptions.Value.BatchSize;
        _claimTimeout = TimeSpan.FromMinutes(outboxOptions.Value.ClaimTimeoutInMin);
    }

    public async Task Execute(CancellationToken cancellationToken = default)
    {
        var processingId = IdentityProvider.New();
        var now = _dateTimeProvider.UtcNow;
        var claimExpiredBefore = now.Subtract(_claimTimeout);

        var claimedMessages = await ClaimMessages(
            processingId,
            now,
            claimExpiredBefore,
            cancellationToken);

        if (claimedMessages.Count == 0)
        {
            return;
        }

        try
        {
            var items = claimedMessages
                .Select(message => new Enqueueable(
                    Payload: message.Content,
                    Subject: message.Type,
                    Id: message.Id,
                    PartitionKey: message.Id,
                    SessionId: message.Id))
                .ToList();

            await _queue.Send(items, cancellationToken);

            var processedOn = _dateTimeProvider.UtcNow;

            foreach (var message in claimedMessages)
            {
                message.ProcessedOn = processedOn;
                message.ProcessingId = null;
                message.ProcessingOn = null;
                message.Error = null;
            }

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Published {OutboxMessageCount} outbox message(s)",
                claimedMessages.Count);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            foreach (var message in claimedMessages)
            {
                message.ProcessingId = null;
                message.ProcessingOn = null;
                message.Error = ex.ToString();
            }

            try
            {
                // If the app is shutting down, the original cancellation token may already be canceled.
                // In that case, using the original token could prevent the processor from releasing the claimed messages.
                // That said, even if this save fails or is skipped during hard shutdown,
                // the design is still safe because the claim has a timeout.
                // The messages will become eligible again once ProcessingOn is older than ClaimTimeout.
                await _context.SaveChangesAsync(CancellationToken.None);
            }
            catch (Exception saveException)
            {
                _logger.LogError(
                    saveException,
                    "Failed to release {OutboxMessageCount} claimed outbox message(s) after publish failure",
                    claimedMessages.Count);
            }

            _logger.LogError(
                ex,
                "Failed to publish {OutboxMessageCount} outbox message(s)",
                claimedMessages.Count);
        }
    }

    private async Task<List<OutboxMessage>> ClaimMessages(
        string processingId,
        DateTime processingOn,
        DateTime claimExpiredBefore,
        CancellationToken cancellationToken)
    {
        return _databaseOptions.Provider switch
        {
            DatabaseProvider.Postgres => await ClaimPostgresMessages(
                processingId,
                processingOn,
                claimExpiredBefore,
                cancellationToken),

            DatabaseProvider.Sqlite => await ClaimSqliteMessages(
                processingId,
                processingOn,
                claimExpiredBefore,
                cancellationToken),

            _ => throw new NotSupportedException(
                $"Database provider '{_databaseOptions.Provider}' is not supported by the outbox processor")
        };
    }

    private async Task<List<OutboxMessage>> ClaimPostgresMessages(
        string processingId,
        DateTime processingOn,
        DateTime claimExpiredBefore,
        CancellationToken cancellationToken)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        var messages = await _context.OutboxMessages
            .FromSqlInterpolated($"""
                                  WITH claimed AS (
                                      SELECT id
                                      FROM outbox_messages
                                      WHERE processed_on IS NULL
                                        AND (
                                            processing_on IS NULL
                                            OR processing_on < {claimExpiredBefore}
                                        )
                                      ORDER BY occurred_on
                                      LIMIT {_batchSize}
                                      FOR UPDATE SKIP LOCKED
                                  )
                                  UPDATE outbox_messages message
                                  SET processing_id = {processingId},
                                      processing_on = {processingOn}
                                  FROM claimed
                                  WHERE message.id = claimed.id
                                  RETURNING message.*
                                  """)
            .ToListAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return messages;
    }

    private async Task<List<OutboxMessage>> ClaimSqliteMessages(
        string processingId,
        DateTime processingOn,
        DateTime claimExpiredBefore,
        CancellationToken cancellationToken)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        var messages = await _context.OutboxMessages
            .FromSqlInterpolated($"""
                                  UPDATE outbox_messages
                                  SET processing_id = {processingId},
                                      processing_on = {processingOn}
                                  WHERE id IN (
                                      SELECT id
                                      FROM outbox_messages
                                      WHERE processed_on IS NULL
                                        AND (
                                            processing_on IS NULL
                                            OR processing_on < {claimExpiredBefore}
                                        )
                                      ORDER BY occurred_on
                                      LIMIT {_batchSize}
                                  )
                                  RETURNING *
                                  """)
            .ToListAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return messages;
    }
}