using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pulse.App.Common.Dispatcher;
using Pulse.Infra.Database.Messaging.Inbox;
using Pulse.Infra.Database.Messaging.Outbox;
using Throw;

namespace Pulse.Infra.Database.Messaging;

public static class Setup
{
    public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureAndValidate<MessagingOptions>(MessagingOptions.Section, configuration);

        var messagingOptions = configuration.GetSection(MessagingOptions.Section).Get<MessagingOptions>();
        messagingOptions.ThrowIfNull();

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

        services.AddScoped<INotificationContext, NotificationContext>();
        services.AddScoped<IInbox, InboxHandler>();

        services.AddScoped<InboxArchiver>();
        services.AddHostedService<InboxArchiverBackgroundService>();

        return services;
    }
}