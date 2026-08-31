using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Pulse.Infra.Plugins;

internal sealed class PluginLoaderHostedService : IHostedService
{
    private const string CorePluginsDirectory = "plugins";

    private readonly ILogger<PluginLoaderHostedService> _logger;
    private readonly PluginOptions _pluginOptions;
    private readonly PluginManager _pluginManager;

    public PluginLoaderHostedService(
        ILogger<PluginLoaderHostedService> logger,
        IOptions<PluginOptions> pluginOptions,
        PluginManager pluginManager)
    {
        _logger = logger;
        _pluginOptions = pluginOptions.Value;
        _pluginManager = pluginManager;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // Load core plugins
        _logger.LogInformation("Loading core plugins...");
        
        var corePluginsPath = Path.Combine(AppContext.BaseDirectory, CorePluginsDirectory);
        var corePluginCount = await _pluginManager.LoadFromDirectoryAsync(corePluginsPath, cancellationToken);

        _logger.LogInformation("Loaded {Count} core plugins", corePluginCount); 
        
        // Load user-provided plugins
        if (string.IsNullOrWhiteSpace(_pluginOptions.RootPath))
        {
            _logger.LogWarning("User plugin root path is not set");
            return;
        }

        _logger.LogInformation("Loading user plugins...");
        
        var userPluginCount = await _pluginManager.LoadFromDirectoryAsync(_pluginOptions.RootPath, cancellationToken);
        
        _logger.LogInformation("Loaded {Count} user plugins", userPluginCount);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}