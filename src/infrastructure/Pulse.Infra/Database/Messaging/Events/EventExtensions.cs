using System.Text.Json;
using Pulse.App.Common.Services.Interfaces;
using Pulse.Domain.Common.Services;
using Pulse.Infra.Database.Contexts;

namespace Pulse.Infra.Database.Messaging.Events;

internal static class EventExtensions
{
    internal static void InsertEvent<T>(
        this DatabaseContext context,
        IDateTimeProvider dateTimeProvider,
        T message) where T : notnull
    {
        var databaseEvent = new Event
        {
            Id = IdentityProvider.New(),
            Type = message.GetType().FullName!,
            Content = JsonSerializer.Serialize(message),
            OccurredOn = dateTimeProvider.UtcNow
        };

        context.Events.Add(databaseEvent);
    }
}