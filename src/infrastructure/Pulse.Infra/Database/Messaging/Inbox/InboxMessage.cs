using System;

namespace Pulse.Infra.Database.Messaging.Inbox;

public sealed class InboxMessage
{
    public required string Id { get; init; }

    public required string Handler { get; init; }

    public DateTime ReceivedOn { get; init; }

    public DateTime? ProcessedOn { get; set; }

    public string? Error { get; set; }
}