namespace Pulse.Plugin;

/// <summary>
/// Static description of a plugin's identity and capabilities.
/// The host must be able to get this WITHOUT triggering any real
/// work — no I/O, no config lookups, no exceptions. It's read to build
/// a catalog independently of whether the plugin is ever actually invoked.
/// </summary>
public sealed record PluginMetadata(
    string Id,
    string DisplayName,
    string Version,
    string Description
);
