using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pulse.App.Common.Services.Interfaces;
using Pulse.Infra.Database;
using Throw;

namespace Pulse.Infra.Plugins;

public static class Setup
{
    public static IServiceCollection AddPlugins(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureAndValidate<PluginOptions>(PluginOptions.Section, configuration);

        var pluginOptions = configuration.GetSection(PluginOptions.Section).Get<PluginOptions>();
        pluginOptions.ThrowIfNull();

        services.AddSingleton<PluginManager>();

        services.AddSingleton<IPluginManager>(provider =>
            provider.GetRequiredService<PluginManager>());

        services.AddHostedService<PluginLoaderHostedService>();

        return services;
    }
}