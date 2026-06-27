namespace Pulse.Database.Outbox.Queues;

public sealed record Dequeueable(string? Payload);