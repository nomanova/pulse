using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Pulse.Database.Outbox;

public static class Setup
{
    public static IServiceCollection AddOutbox(this IServiceCollection services, IConfiguration configuration)
    {
        // TODO
        
        services.AddTransient<IEventBus, EventBus>();
        
        return services;
    }
}