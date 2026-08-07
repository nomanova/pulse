using Microsoft.Extensions.DependencyInjection;

namespace Pulse.Infra.Security.Authentication.ApiKey;

public static class Setup
{
    public static IServiceCollection AddApiKeyAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication()
            .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>("ApiKey", _ => { });
        
        return services;
    }
}