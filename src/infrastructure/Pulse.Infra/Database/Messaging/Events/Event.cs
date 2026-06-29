using System;

namespace Pulse.Infra.Database.Messaging.Events;

public sealed class Event
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