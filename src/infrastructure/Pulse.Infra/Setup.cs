using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pulse.App.Common.Services.Interfaces;
using Pulse.Database;
using Pulse.Infra.Database;
using Pulse.Infra.Security;
using Pulse.Infra.Services;
using Throw;

namespace Pulse.Infra;

public static class Setup
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.ThrowIfNull();
        configuration.ThrowIfNull();

        services.AddConfiguration();
        services.AddHttpContextAccessor();
        services.AddServices();

        services
            .AddDatabase(configuration)
            .AddSecurity(configuration);

        return services;
    }

    private static void AddConfiguration(this IServiceCollection services)
    {
        services.AddOptions();
    }

    private static void AddServices(this IServiceCollection services)
    {
        services.AddSingleton<IEnvironmentProvider, EnvironmentProvider>();
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
    }
}