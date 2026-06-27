using System.ComponentModel.DataAnnotations;

namespace Pulse.Infra.Database.Outbox;

public sealed record OutboxOptions
{
    public const string Section = "Outbox";
    
    public enum QueueType
    {
        InMemory
    }
    
    [Required] public QueueType Queue { get; set; }

    [Range(1, 60)]
    public uint FrequencyInSec { get; set; } = 5;
    
    [Range(1, 1000)]
    public uint BatchSize { get; set; } = 100;

    [Range(1, 60)]
    public uint ClaimTimeoutInMin { get; set; } = 5;
}