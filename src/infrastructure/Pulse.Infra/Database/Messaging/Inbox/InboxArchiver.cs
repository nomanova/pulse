using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pulse.App.Common.Services.Interfaces;
using Pulse.Infra.Database.Contexts;

namespace Pulse.Infra.Database.Messaging.Inbox;

public class InboxArchiver : MessageArchiver
{
    protected override string TableName => "inbox_messages";

    protected override string MessageKind => "inbox";

    public InboxArchiver(
        IDateTimeProvider dateTimeProvider,
        DatabaseContext context,
        IOptions<DatabaseOptions> databaseOptions,
        IOptions<MessagingOptions.InboxOptions> inboxOptions,
        ILogger<InboxArchiver> logger)
        : base(
            dateTimeProvider,
            context,
            databaseOptions.Value,
            logger,
            inboxOptions.Value.ArchiveBatchSize,
            TimeSpan.FromMinutes(inboxOptions.Value.ArchiveTimeoutInMin))
    {
    }
}