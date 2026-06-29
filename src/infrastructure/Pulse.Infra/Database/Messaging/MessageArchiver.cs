using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pulse.App.Common.Services.Interfaces;
using Pulse.Infra.Database.Contexts;

namespace Pulse.Infra.Database.Messaging;

public abstract class MessageArchiver
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly DatabaseContext _context;
    private readonly DatabaseOptions _databaseOptions;
    private readonly ILogger _logger;

    private readonly uint _batchSize;
    private readonly TimeSpan _archiveTimeout;

    protected MessageArchiver(
        IDateTimeProvider dateTimeProvider,
        DatabaseContext context,
        DatabaseOptions databaseOptions,
        ILogger logger,
        uint batchSize,
        TimeSpan archiveTimeout)
    {
        _dateTimeProvider = dateTimeProvider;
        _context = context;
        _databaseOptions = databaseOptions;
        _logger = logger;
        _batchSize = batchSize;
        _archiveTimeout = archiveTimeout;
    }

    protected abstract string TableName { get; }

    protected abstract string MessageKind { get; }

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
                $"Database provider '{_databaseOptions.Provider}' is not supported by the {MessageKind} archiver")
        };

        if (archivedCount > 0)
        {
            _logger.LogInformation("Archived {MessageCount} {MessageKind} message(s)", archivedCount, MessageKind);
        }
    }

    private async Task<int> ArchivePostgresMessages(
        DateTime processedBefore,
        CancellationToken cancellationToken)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        var sql = string.Format(
            """
            WITH archived AS (
                SELECT id
                FROM {0}
                WHERE processed_on IS NOT NULL
                  AND processed_on < {1}
                ORDER BY processed_on
                LIMIT {2}
                FOR UPDATE SKIP LOCKED
            )
            DELETE FROM {0} message
            USING archived
            WHERE message.id = archived.id
            """,
            TableName,
            "{0}",
            "{1}");

        var archivedCount = await _context.Database.ExecuteSqlRawAsync(
            sql,
            [processedBefore, _batchSize],
            cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return archivedCount;
    }

    private async Task<int> ArchiveSqliteMessages(
        DateTime processedBefore,
        CancellationToken cancellationToken)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        var sql = string.Format(
            """
            DELETE FROM {0}
            WHERE id IN (
                SELECT id
                FROM {0}
                WHERE processed_on IS NOT NULL
                  AND processed_on < {1}
                ORDER BY processed_on
                LIMIT {2}
            )
            """,
            TableName,
            "{0}",
            "{1}");

        var archivedCount = await _context.Database.ExecuteSqlRawAsync(
            sql,
            [processedBefore, _batchSize],
            cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return archivedCount;
    }
}