using Microsoft.Extensions.DependencyInjection;

namespace Pulse.Web.Core.ViewModels.Applications;

public static class Setup
{
    public static IServiceCollection AddApplicationViewModels(this IServiceCollection services)
    {
        services.AddTransient<ApplicationsViewModel>();
        services.AddTransient<AddApplicationViewModel>();
        services.AddTransient<ApplicationDetailViewModel>();

        return services;
    }
}