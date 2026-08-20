namespace Pulse.Plugin;

public sealed record PluginMetadata(
    string Id,
    string DisplayName,
    string Version,
    string Description
);
