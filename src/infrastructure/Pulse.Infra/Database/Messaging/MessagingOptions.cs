using System.ComponentModel.DataAnnotations;

namespace Pulse.Infra.Database.Messaging;

public sealed record MessagingOptions
{
    public const string Section = "Messaging";

    public enum QueueType
    {
        InMemory
    }

    public sealed record OutboxOptions
    {
        public const string Section = "Outbox";

        [Range(1, 60)] public uint ProcessFrequencyInSec { get; set; }

        [Range(1, 1000)] public uint ProcessBatchSize { get; set; }

        [Range(1, 60)] public uint ClaimTimeoutInMin { get; set; }

        [Range(1, 3600)] public uint ArchiveFrequencyInSec { get; set; }

        [Range(1, 1000)] public uint ArchiveBatchSize { get; set; }

        [Range(1, 10080)] public uint ArchiveTimeoutInMin { get; set; }
    }

    public sealed record InboxOptions
    {
        public const string Section = "Inbox";
    }

    [Required] public QueueType Queue { get; set; }

    [Required] public OutboxOptions? Outbox { get; set; }

    //[Required] public InboxOptions? Inbox { get; set; }
}