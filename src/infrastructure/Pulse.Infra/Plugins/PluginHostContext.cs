using System;
using Microsoft.Extensions.Logging;
using Pulse.Plugin;

namespace Pulse.Infra.Plugins;

public sealed class PluginHostContext : IPluginHostContext
{
    private readonly ILogger _logger;
    private readonly string _pluginId;

    public PluginHostContext(ILogger logger, string pluginId)
    {
        _logger = logger;
        _pluginId = pluginId;
    }

    public void Log(PluginLogLevel level, string message)
    {
        var logLevel = Enum.TryParse<LogLevel>(level.ToString(), ignoreCase: true, out var parsed)
            ? parsed
            : LogLevel.Information;

        _logger.Log(logLevel, "[plugin:{PluginId}] {Message}", _pluginId, message);
    }
}