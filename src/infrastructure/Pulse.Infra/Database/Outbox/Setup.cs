using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pulse.Infra.Database.Outbox.Queues;
using Throw;

namespace Pulse.Infra.Database.Outbox;

public static class Setup
{
    public static IServiceCollection AddOutbox(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureAndValidate<OutboxOptions>(OutboxOptions.Section, configuration);
        
        var outboxOptions = configuration.GetSection(OutboxOptions.Section).Get<OutboxOptions>();
        outboxOptions.ThrowIfNull();

        switch (outboxOptions.Queue)
        {
            case OutboxOptions.QueueType.InMemory:
                services.AddSingleton<IQueue, InMemoryQueue>();
                break;
            default:
                throw new NotImplementedException();
        }
    
        services.AddScoped<OutboxProcessor>();
        services.AddHostedService<OutboxBackgroundService>();
        
        return services;
    }
}