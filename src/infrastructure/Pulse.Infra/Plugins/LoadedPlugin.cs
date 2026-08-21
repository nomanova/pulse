using Pulse.Plugin;

namespace Pulse.Infra.Plugins;

public sealed record LoadedPlugin
{
    public required PluginLoadContext LoadContext { get; init; }
    
    public required IPlugin Instance { get; init; }
}