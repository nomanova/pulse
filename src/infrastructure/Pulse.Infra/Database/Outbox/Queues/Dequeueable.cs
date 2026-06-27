namespace Pulse.Infra.Database.Outbox.Queues;

public sealed record Dequeueable(string? Payload);