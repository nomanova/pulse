using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pulse.Domain.Aggregates.Users.Services;
using Pulse.Infra.Security.Authentication;
using Pulse.Infra.Security.Authorization;
using Pulse.Infra.Security.Cors;
using Pulse.Infra.Security.DataProtection;
using Pulse.Infra.Security.Password;
using Throw;

namespace Pulse.Infra.Security;

public static class Setup
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddSecurity(IConfiguration configuration)
        {
            services.ThrowIfNull();
            configuration.ThrowIfNull();
        
            services
                .AddAppDataProtection(configuration)
                .AddAppUserServices()
                .AddAppAuthentication(configuration)
                .AddAppCors(configuration)
                .AddAppAuthorization();
        
            return services;
        }

        private IServiceCollection AddAppUserServices()
        {
            services.AddSingleton<IUserPasswordHasher, BCryptPasswordHasher>();
        
            return services;
        }
    }
}