using System.ComponentModel.DataAnnotations;

namespace Pulse.Database.Outbox;

public sealed record OutboxOptions
{
    public const string Section = "Outbox";
    
    public enum QueueType
    {
        InMemory
    }
    
    [Required] public QueueType Queue { get; set; }
}