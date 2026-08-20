namespace Pulse.Plugin;

public sealed record PluginParameterDefinition(
    string Key, string Name, string? Description = null, bool IsRequired = true);