using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Pulse.App.Common.Services.Interfaces;
using Pulse.Plugin;

namespace Pulse.Infra.Plugins;

public sealed class PluginManager : IPluginManager
{
    private const int PluginInitializeTimeoutInSeconds = 10;

    private readonly Dictionary<string, LoadedPlugin> _plugins = new(StringComparer.OrdinalIgnoreCase);
    private readonly ILogger<PluginManager> _logger;

    public PluginManager(ILogger<PluginManager> logger)
    {
        _logger = logger;
    }

    public IReadOnlyCollection<PluginMetadata> Catalog => _plugins.Values.Select(p => p.Metadata).ToList();

    public async Task<PluginInvocationResult> InvokeAsync(string pluginId, PluginInvocationRequest request,
        CancellationToken cancellationToken)
    {
        if (!_plugins.TryGetValue(pluginId, out var plugin))
        {
            return PluginInvocationResult.Fail($"Unknown plugin '{pluginId}'");
        }

        try
        {
            return await plugin.Instance.InvokeAsync(request, cancellationToken);
        }
        catch (Exception ex)
        {
            // Never let a plugin exception bubble unhandled
            _logger.LogError(ex, "Plugin {Id} threw during InvokeAsync", pluginId);
            return PluginInvocationResult.Fail($"Plugin '{pluginId}' failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Convention: each plugin lives in its own subfolder named after its
    /// main DLL, e.g., plugins/SamplePlugin/SamplePlugin.dll (+ its private
    /// dependency DLLs alongside it). One folder = one isolated ALC.
    /// </summary>
    internal async Task LoadFromDirectoryAsync(string pluginsRootPath, CancellationToken cancellationToken = default)
    {
        if (!Directory.Exists(pluginsRootPath))
        {
            _logger.LogWarning("Plugins directory {Path} does not exist, skipping load", pluginsRootPath);
            return;
        }

        foreach (var pluginFolder in Directory.GetDirectories(pluginsRootPath))
        {
            var folderName = Path.GetFileName(pluginFolder);
            var dllPath = Path.Combine(pluginFolder, folderName + ".dll");

            if (!File.Exists(dllPath))
            {
                _logger.LogWarning("Skipping {Folder}: expected {Dll} not found", folderName, dllPath);
                continue;
            }

            try
            {
                await LoadPluginAsync(dllPath, cancellationToken);
            }
            catch (Exception ex)
            {
                // A broken/malicious plugin must never take the host down at startup.
                _logger.LogError(ex, "Failed to load plugin from {Dll}", dllPath);
            }
        }
    }

    private async Task LoadPluginAsync(string dllPath, CancellationToken cancellationToken)
    {
        var loadContext = new PluginLoadContext(dllPath, collectible: true);
        var assembly = loadContext.LoadFromAssemblyPath(dllPath);

        var pluginType = assembly.GetTypes()
            .SingleOrDefault(t =>
                typeof(IPlugin).IsAssignableFrom(t) && t is { IsAbstract: false, IsInterface: false });

        if (pluginType is null)
        {
            throw new InvalidOperationException($"No IPlugin implementation found in {dllPath}.");
        }

        if (Activator.CreateInstance(pluginType) is not IPlugin instance)
        {
            throw new InvalidOperationException($"Could not construct {pluginType.FullName} in {dllPath}.");
        }

        var metadata = instance.Metadata;

        if (_plugins.ContainsKey(metadata.Id))
        {
            throw new InvalidOperationException($"Duplicate plugin id '{metadata.Id}' from {dllPath}.");
        }

        var hostContext = new PluginHostContext(_logger, metadata.Id);

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(TimeSpan.FromSeconds(PluginInitializeTimeoutInSeconds));

        await instance.InitializeAsync(hostContext, cts.Token);

        _plugins[metadata.Id] = new LoadedPlugin
        {
            Metadata = metadata,
            Instance = instance,
            LoadContext = loadContext
        };

        _logger.LogDebug("Loaded plugin {Id} v{Version} from {Dll}", metadata.Id, metadata.Version, dllPath);
    }
}