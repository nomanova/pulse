using System.ComponentModel.DataAnnotations;

namespace Pulse.Infra.Database.Messaging;

public sealed record MessagingOptions
{
    public const string Section = "Messaging";

    public sealed record EventOptions
    {
        public const string Section = "Events";

        [Range(1, 60)] public uint ProcessFrequencyInSec { get; set; }

        [Range(1, 1000)] public uint ProcessBatchSize { get; set; }

        [Range(1, 60)] public uint ClaimTimeoutInMin { get; set; }

        [Range(1, 3600)] public uint ArchiveFrequencyInSec { get; set; }

        [Range(1, 1000)] public uint ArchiveBatchSize { get; set; }

        [Range(1, 10080)] public uint ArchiveTimeoutInMin { get; set; }
    }

    [Required] public EventOptions? Events { get; set; }
}