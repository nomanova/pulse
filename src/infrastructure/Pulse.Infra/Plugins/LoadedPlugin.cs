using Pulse.Plugin;

namespace Pulse.Infra.Plugins;

public sealed class LoadedPlugin
{
    public required PluginMetadata Metadata { get; init; }
    
    public required IPlugin Instance { get; init; }
    
    public required PluginLoadContext LoadContext { get; init; }
}