using Microsoft.Extensions.DependencyInjection;

namespace Pulse.Application;

public static class Setup
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services;
    }
}