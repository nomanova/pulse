using System;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Pulse.Plugin;

namespace Pulse.Infra.Plugins;

public sealed class PluginHostContext : IPluginHostContext
{
    private readonly ILogger _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _pluginId;

    public PluginHostContext(ILogger logger, IHttpClientFactory httpClientFactory, string pluginId)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _pluginId = pluginId;
    }

    public void Log(PluginLogLevel level, string message)
    {
        var logLevel = Enum.TryParse<LogLevel>(level.ToString(), ignoreCase: true, out var parsed)
            ? parsed
            : LogLevel.Information;

        _logger.Log(logLevel, "[plugin:{PluginId}] {Message}", _pluginId, message);
    }

    public HttpClient HttpClient() => _httpClientFactory.CreateClient(_pluginId);
}