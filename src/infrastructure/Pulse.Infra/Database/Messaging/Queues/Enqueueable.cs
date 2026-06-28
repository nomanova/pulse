namespace Pulse.Infra.Database.Messaging.Queues;

public sealed record Enqueueable(string Payload, string Subject, string Id, string PartitionKey, string SessionId);