using System.Text.Json;
using Pulse.App.Common.Services.Interfaces;
using Pulse.Domain.Common.Services;
using Pulse.Infra.Database.Contexts;

namespace Pulse.Infra.Database.Messaging.Outbox;

internal static class OutboxExtensions
{
    internal static void InsertOutboxMessage<T>(
        this DatabaseContext context, 
        IDateTimeProvider dateTimeProvider, 
        T message) where T : notnull
    {
        var outboxMessage = new OutboxMessage
        {
            Id = IdentityProvider.New(),
            Type = message.GetType().FullName!,
            Content = JsonSerializer.Serialize(message),
            OccurredOn = dateTimeProvider.UtcNow
        };
      
        context.OutboxMessages.Add(outboxMessage);
    }
}