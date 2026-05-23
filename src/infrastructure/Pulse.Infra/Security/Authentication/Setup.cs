using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pulse.Infra.Security.Authentication.Jwt;

namespace Pulse.Infra.Security.Authentication;

public static class Setup
{
    public static IServiceCollection AddAppAuthentication(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddJwtAuthentication(configuration);
        
        return services;
    }
}