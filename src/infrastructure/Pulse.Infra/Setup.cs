using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pulse.App.Common.Services.Interfaces;
using Pulse.Infra.Database;
using Pulse.Infra.Plugins;
using Pulse.Infra.Security;
using Pulse.Infra.Services;
using Throw;

namespace Pulse.Infra;

public static class Setup
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddInfrastructure(IConfiguration configuration)
        {
            services.ThrowIfNull();
            configuration.ThrowIfNull();

            services.AddConfiguration();
            services.AddHttpContextAccessor();
            services.AddServices();

            services
                .AddDatabase(configuration)
                .AddSecurity(configuration)
                .AddPlugins(configuration);

            return services;
        }

        private void AddConfiguration()
        {
            services.AddOptions();
        }

        private void AddServices()
        {
            services.AddSingleton<IEnvironmentProvider, EnvironmentProvider>();
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        }
    }
}