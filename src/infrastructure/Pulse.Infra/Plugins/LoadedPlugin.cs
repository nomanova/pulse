using Pulse.Domain.Common.Models.Enums;
using Pulse.Plugin;
using Pulse.Plugin.Providers;

namespace Pulse.Infra.Plugins;

public abstract class LoadedPlugin
{
    public required PluginMetadata Metadata { get; init; }
    
    public required PluginLoadContext LoadContext { get; init; }
}

public sealed class LoadedProviderPlugin : LoadedPlugin
{
    public required IProviderPlugin Instance { get; init; }
    
    public required Channel Channel { get; init; }
}