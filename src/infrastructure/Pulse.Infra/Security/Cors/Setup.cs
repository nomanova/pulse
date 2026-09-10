using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pulse.Infra.Database;
using Throw;

namespace Pulse.Infra.Security.Cors;

public static class Setup
{
    public static IServiceCollection AddAppCors(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureAndValidate<CorsOptions>(CorsOptions.Section, configuration);

        var options = configuration.GetSection(CorsOptions.Section).Get<CorsOptions>();
        options.ThrowIfNull();

        services.AddCors(corsOptions =>
        {
            corsOptions.AddDefaultPolicy(builder =>
            {
                builder.WithOrigins(options.AllowedOrigins?.ToArray() ?? [])
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        return services;
    }
}