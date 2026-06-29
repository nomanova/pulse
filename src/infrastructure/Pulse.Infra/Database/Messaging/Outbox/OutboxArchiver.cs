using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pulse.App.Common.Services.Interfaces;
using Pulse.Infra.Database.Contexts;

namespace Pulse.Infra.Database.Messaging.Outbox;

public sealed class OutboxArchiver : MessageArchiver
{
    protected override string TableName => "outbox_messages";

    protected override string MessageKind => "outbox";

    public OutboxArchiver(
        IDateTimeProvider dateTimeProvider,
        DatabaseContext context,
        IOptions<DatabaseOptions> databaseOptions,
        IOptions<MessagingOptions.OutboxOptions> outboxOptions,
        ILogger<OutboxArchiver> logger)
        : base(
            dateTimeProvider,
            context,
            databaseOptions.Value,
            logger,
            outboxOptions.Value.ArchiveBatchSize,
            TimeSpan.FromMinutes(outboxOptions.Value.ArchiveTimeoutInMin))
    {
    }
}