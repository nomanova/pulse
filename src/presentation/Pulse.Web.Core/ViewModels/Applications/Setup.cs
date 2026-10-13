using Microsoft.Extensions.DependencyInjection;

namespace Pulse.Web.Core.ViewModels.Applications;

public static class Setup
{
    public static IServiceCollection AddApplicationViewModels(this IServiceCollection services)
    {
        services.AddTransient<ApplicationListViewModel>();
        services.AddTransient<AddApplicationViewModel>();

        services.AddTransient<ApplicationViewModel>();

        return services;
    }
}