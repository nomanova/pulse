using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pulse.App.Common.Services.Interfaces;
using Pulse.Infra.Database.Contexts;

namespace Pulse.Infra.Database.Messaging.Outbox;

public sealed class OutboxArchiver
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly DatabaseContext _context;
    private readonly DatabaseOptions _databaseOptions;
    private readonly ILogger<OutboxArchiver> _logger;

    private readonly uint _batchSize;
    private readonly TimeSpan _archiveTimeout;

    public OutboxArchiver(
        IDateTimeProvider dateTimeProvider,
        DatabaseContext context,
        IOptions<DatabaseOptions> databaseOptions,
        IOptions<MessagingOptions.OutboxOptions> outboxOptions,
        ILogger<OutboxArchiver> logger)
    {
        _dateTimeProvider = dateTimeProvider;
        _context = context;
        _databaseOptions = databaseOptions.Value;
        _logger = logger;

        _batchSize = outboxOptions.Value.ArchiveBatchSize;
        _archiveTimeout = TimeSpan.FromMinutes(outboxOptions.Value.ArchiveTimeoutInMin);
    }

    public async Task Execute(CancellationToken cancellationToken = default)
    {
        var processedBefore = _dateTimeProvider.UtcNow.Subtract(_archiveTimeout);

        var archivedCount = _databaseOptions.Provider switch
        {
            DatabaseProvider.Postgres => await ArchivePostgresMessages(
                processedBefore,
                cancellationToken),

            DatabaseProvider.Sqlite => await ArchiveSqliteMessages(
                processedBefore,
                cancellationToken),

            _ => throw new NotSupportedException(
                $"Database provider '{_databaseOptions.Provider}' is not supported by the outbox archiver")
        };

        if (archivedCount > 0)
        {
            _logger.LogInformation(
                "Archived {OutboxMessageCount} outbox message(s)",
                archivedCount);
        }
    }

    private async Task<int> ArchivePostgresMessages(
        DateTime processedBefore,
        CancellationToken cancellationToken)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        var archivedCount = await _context.Database.ExecuteSqlInterpolatedAsync($"""
             WITH archived AS (
                 SELECT id
                 FROM outbox_messages
                 WHERE processed_on IS NOT NULL
                   AND processed_on < {processedBefore}
                 ORDER BY processed_on
                 LIMIT {_batchSize}
                 FOR UPDATE SKIP LOCKED
             )
             DELETE FROM outbox_messages message
             USING archived
             WHERE message.id = archived.id
             """, cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return archivedCount;
    }

    private async Task<int> ArchiveSqliteMessages(
        DateTime processedBefore,
        CancellationToken cancellationToken)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        var archivedCount = await _context.Database.ExecuteSqlInterpolatedAsync($"""
             DELETE FROM outbox_messages
             WHERE id IN (
                 SELECT id
                 FROM outbox_messages
                 WHERE processed_on IS NOT NULL
                   AND processed_on < {processedBefore}
                 ORDER BY processed_on
                 LIMIT {_batchSize}
             )
             """, cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return archivedCount;
    }
}