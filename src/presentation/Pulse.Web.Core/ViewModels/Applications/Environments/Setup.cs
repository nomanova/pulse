using Microsoft.Extensions.DependencyInjection;

namespace Pulse.Web.Core.ViewModels.Applications.Environments;

public static class Setup
{
    public static IServiceCollection AddEnvironmentViewModels(this IServiceCollection services)
    {
        services.AddTransient<EnvironmentViewModel>();
        
        services.AddTransient<EnvironmentContainerViewModel>();
        
        services.AddTransient<EnvironmentApiKeysViewModel>();
        services.AddTransient<EnvironmentConnectionsViewModel>();
        
        return services;
    }
}