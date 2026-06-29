using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Common.Services.Interfaces;
using Pulse.Domain.Common.Models.Events;
using Pulse.Domain.Common.Services;
using Pulse.Infra.Database.Contexts;

namespace Pulse.Infra.Database.Messaging.Outbox;

/**
 * The implementation provides at-least-once processing with a single active claim per message.
 * To make message handling effectively exactly once, each handler should be made idempotent by
 * means of an inbox mechanism.
 */
public sealed class OutboxProcessor
{
    private readonly ILogger<OutboxProcessor> _logger;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly DatabaseContext _context;
    private readonly DatabaseOptions _databaseOptions;
    private readonly INotificationContext _notificationContext;
    private readonly IPublisher _publisher;

    private readonly uint _batchSize;
    private readonly TimeSpan _claimTimeout;

    public OutboxProcessor(
        ILogger<OutboxProcessor> logger,
        IDateTimeProvider dateTimeProvider,
        DatabaseContext context,
        IOptions<DatabaseOptions> databaseOptions,
        IOptions<MessagingOptions.OutboxOptions> outboxOptions,
        INotificationContext notificationContext,
        IPublisher publisher)
    {
        _logger = logger;
        _dateTimeProvider = dateTimeProvider;
        _context = context;
        _databaseOptions = databaseOptions.Value;
        _notificationContext = notificationContext;
        _publisher = publisher;

        _batchSize = outboxOptions.Value.ProcessBatchSize;
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

        foreach (var message in claimedMessages)
        {
            try
            {
                await PublishEvent(message, cancellationToken);

                message.ProcessedOn = _dateTimeProvider.UtcNow;
                message.ProcessingId = null;
                message.ProcessingOn = null;
                message.Error = null;

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Processed event {EventId} of type {EventType}",
                    message.Id,
                    message.Type);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                message.ProcessingId = null;
                message.ProcessingOn = null;
                message.Error = ex.ToString();

                try
                {
                    await _context.SaveChangesAsync(CancellationToken.None);
                }
                catch (Exception saveException)
                {
                    _logger.LogError(
                        saveException,
                        "Failed to release claimed event {EventId} after handler failure",
                        message.Id);
                }

                _logger.LogError(
                    ex,
                    "Failed to process event {EventId} of type {EventType}",
                    message.Id,
                    message.Type);
            }
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

        var events = await _context.OutboxMessages
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

        return events;
    }

    private async Task PublishEvent(
        OutboxMessage message,
        CancellationToken cancellationToken)
    {
        var eventType = ResolveEventType(message.Type);

        if (eventType is null)
        {
            throw new InvalidOperationException(
                $"Event type '{message.Type}' could not be resolved.");
        }

        if (!typeof(INotification).IsAssignableFrom(eventType))
        {
            throw new InvalidOperationException(
                $"Event type '{message.Type}' does not implement {nameof(INotification)}.");
        }

        var notification = JsonSerializer.Deserialize(message.Content, eventType);

        if (notification is null)
        {
            throw new InvalidOperationException(
                $"Event '{message.Id}' of type '{message.Type}' could not be deserialized.");
        }

        try
        {
            _notificationContext.SetNotificationId(message.Id);
            
            await PublishDynamic((dynamic)notification, cancellationToken);
        }
        finally
        {
            _notificationContext.Clear();
        }
    }

    private static Type? ResolveEventType(string eventType)
    {
        return Type.GetType(eventType, throwOnError: false)
               ?? AppDomain.CurrentDomain
                   .GetAssemblies()
                   .Select(assembly => assembly.GetType(eventType, throwOnError: false))
                   .FirstOrDefault(type => type is not null);
    }

    private Task PublishDynamic<TNotification>(
        TNotification notification,
        CancellationToken cancellationToken)
        where TNotification : INotification
    {
        return _publisher.Publish(notification, cancellationToken);
    }
}