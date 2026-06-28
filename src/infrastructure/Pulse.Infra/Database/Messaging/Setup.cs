using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pulse.Infra.Database.Messaging.Outbox;
using Pulse.Infra.Database.Messaging.Queues;
using Throw;

namespace Pulse.Infra.Database.Messaging;

public static class Setup
{
    public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureAndValidate<MessagingOptions>(MessagingOptions.Section, configuration);

        var messagingOptions = configuration.GetSection(MessagingOptions.Section).Get<MessagingOptions>();
        messagingOptions.ThrowIfNull();

        switch (messagingOptions.Queue)
        {
            case MessagingOptions.QueueType.InMemory:
                services.AddSingleton<IQueue, InMemoryQueue>();
                break;
            default:
                throw new NotImplementedException();
        }

        // Outbox
        const string outboxSection =
            $"{MessagingOptions.Section}:{MessagingOptions.OutboxOptions.Section}";
        services.ConfigureAndValidate<MessagingOptions.OutboxOptions>(outboxSection, configuration);

        services.AddScoped<OutboxProcessor>();
        services.AddHostedService<OutboxProcessorBackgroundService>();

        services.AddScoped<OutboxArchiver>();
        services.AddHostedService<OutboxArchiverBackgroundService>();

        // Inbox
        const string inboxSection =
            $"{MessagingOptions.Section}:{MessagingOptions.InboxOptions.Section}";
        services.ConfigureAndValidate<MessagingOptions.InboxOptions>(inboxSection, configuration);

        // TODO

        return services;
    }
}