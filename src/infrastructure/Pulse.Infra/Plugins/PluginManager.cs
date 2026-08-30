using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using Microsoft.Extensions.Logging;
using Pulse.App.Common.Services.Interfaces;
using Pulse.Domain.Channels;
using Pulse.Plugin;
using Pulse.Plugin.Providers;

namespace Pulse.Infra.Plugins;

public sealed class PluginManager : IPluginManager
{
    private const int PluginInitializeTimeoutInSeconds = 10;

    private readonly Dictionary<string, LoadedPlugin> _plugins = new(StringComparer.OrdinalIgnoreCase);

    private readonly ILogger<PluginManager> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    public PluginManager(ILogger<PluginManager> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    public IReadOnlyCollection<IPlugin> GetCatalog(Channel? channel = null)
    {
        if (channel == null)
        {
            return _plugins.Values.Select(p => p.Instance).ToList();
        }

        return _plugins.Values
            .Select(p => p.Instance)
            .OfType<IProviderPlugin>()
            .Where(p => p.Channel == channel)
            .ToList();
    }

    public IPlugin? TryGet(string pluginId)
    {
        return !_plugins.TryGetValue(pluginId, out var plugin) ? null : plugin.Instance;
    }

    public async Task<ErrorOr<Success>> Invoke(string pluginId, PluginInvocationRequest request,
        CancellationToken cancellationToken)
    {
        if (!_plugins.TryGetValue(pluginId, out var plugin))
        {
            return Error.NotFound(description: $"Unknown plugin '{pluginId}'");
        }

        try
        {
            await plugin.Instance.Invoke(request, cancellationToken);
        }
        catch (Exception ex)
        {
            // Never let a plugin exception bubble unhandled
            _logger.LogError(ex, "Plugin {Id} threw during invocation", pluginId);
            return Error.Failure(description: $"Plugin '{pluginId}' failed: {ex.Message}");
        }

        return Result.Success;
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
                // A broken/malicious plugin must not take the host down at startup.
                _logger.LogError(ex, "Failed to load plugin from {Dll}", dllPath);
            }
        }
    }

    private async Task LoadPluginAsync(string dllPath, CancellationToken cancellationToken)
    {
        var loadContext = new PluginLoadContext(dllPath, collectible: true);
        var assembly = loadContext.LoadFromAssemblyPath(dllPath);

        IProviderPlugin instance;

        if (TryGetPluginType(typeof(IProviderPlugin), assembly, out var pluginType))
        {
            instance = ActivatePlugin<IProviderPlugin>(pluginType!, dllPath);
        }
        else
        {
            throw new InvalidOperationException($"No valid plugin implementation found in {dllPath}.");
        }

        var metadata = instance.Metadata;

        if (_plugins.ContainsKey(metadata.Id))
        {
            throw new InvalidOperationException($"Duplicate plugin id '{metadata.Id}' from {dllPath}.");
        }

        var hostContext = new PluginHostContext(_logger, _httpClientFactory, metadata.Id);

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(TimeSpan.FromSeconds(PluginInitializeTimeoutInSeconds));

        await instance.Initialize(hostContext, cts.Token);

        _plugins[metadata.Id] = new LoadedPlugin
        {
            LoadContext = loadContext,
            Instance = instance
        };

        _logger.LogDebug("Loaded plugin {Id} v{Version} from {Dll}", metadata.Id, metadata.Version, dllPath);
    }

    private static T ActivatePlugin<T>(Type type, string dllPath)
    {
        return Activator.CreateInstance(type) is not T instance
            ? throw new InvalidOperationException($"Could not construct {type.FullName} in {dllPath}.")
            : instance;
    }

    private static bool TryGetPluginType(Type type, Assembly assembly, out Type? pluginType)
    {
        pluginType = assembly.GetTypes()
            .SingleOrDefault(t =>
                type.IsAssignableFrom(t) && t is { IsAbstract: false, IsInterface: false });

        return pluginType != null;
    }
}