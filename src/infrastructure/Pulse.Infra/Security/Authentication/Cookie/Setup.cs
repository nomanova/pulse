using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pulse.Infra.Database;

namespace Pulse.Infra.Security.Authentication.Cookie;

public static class Setup
{
    public static IServiceCollection AddCookieAuthentication(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.ConfigureAndValidate<CookieOptions>(CookieOptions.Section, configuration);
        
        return services;
    }
}