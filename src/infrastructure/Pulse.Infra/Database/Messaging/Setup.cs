using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pulse.Infra.Database.Messaging.Events;
using Throw;

namespace Pulse.Infra.Database.Messaging;

public static class Setup
{
    public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureAndValidate<MessagingOptions>(MessagingOptions.Section, configuration);

        var messagingOptions = configuration.GetSection(MessagingOptions.Section).Get<MessagingOptions>();
        messagingOptions.ThrowIfNull();

        // Events
        const string eventsSection =
            $"{MessagingOptions.Section}:{MessagingOptions.EventOptions.Section}";
        services.ConfigureAndValidate<MessagingOptions.EventOptions>(eventsSection, configuration);

        services.AddScoped<EventProcessor>();
        services.AddHostedService<EventProcessorBackgroundService>();

        services.AddScoped<EventArchiver>();
        services.AddHostedService<EventArchiverBackgroundService>();

        return services;
    }
}