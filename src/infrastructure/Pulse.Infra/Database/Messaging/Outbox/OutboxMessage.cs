using System;

namespace Pulse.Infra.Database.Messaging.Outbox;

public sealed class OutboxMessage
{
    public required string Id { get; init; }

    public required string Type { get; init; }
    
    public required string Content { get; init; }
    
    public DateTime OccurredOn { get; init; }
    
    public DateTime? ProcessingOn { get; set; }

    public string? ProcessingId { get; set; }
    
    public DateTime? ProcessedOn { get; set; }
    
    public string? Error { get; set; }
}