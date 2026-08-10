namespace Pulse.Plugin;

/// <summary>
/// Narrow, stable surface the host exposes back to plugins.
/// Keep this small and additive-only across versions: a plugin built
/// against Contracts 1.0 should still run unmodified against a host
/// using Contracts 1.4. Never remove or change the signature of an
/// existing member — add a new member instead, or introduce
/// IPluginHostContextV2 and have the host implement both.
/// </summary>
public interface IPluginHostContext
{
    void Log(PluginLogLevel level, string message);
}