using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pulse.Domain.Aggregates.Users.Services;
using Pulse.Infra.Security.Authentication;
using Pulse.Infra.Security.DataProtection;
using Pulse.Infra.Security.Password;
using Throw;

namespace Pulse.Infra.Security;

public static class Setup
{
    public static IServiceCollection AddSecurity(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.ThrowIfNull();
        configuration.ThrowIfNull();
        
        services
            .AddAppDataProtection(configuration)
            .AddAppUserServices()
            .AddAppAuthentication(configuration);
        
        return services;
    }

    private static IServiceCollection AddAppUserServices(this IServiceCollection services)
    {
        services.AddSingleton<IUserPasswordHasher, BCryptPasswordHasher>();
        
        return services;
    }
}