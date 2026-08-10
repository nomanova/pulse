using System.Threading;
using System.Threading.Tasks;

namespace Pulse.Plugin;

/// <summary>
/// Contract every plugin assembly must implement. The sample host's
/// discovery logic assumes exactly one IPlugin implementation per DLL.
/// </summary>
public interface IPlugin
{
    /// <summary>
    /// Must be inexpensive and side effect free. The host may read this to
    /// build a catalog without ever calling InitializeAsync or InvokeAsync,
    /// so don't do config validation, network calls, or file I/O here.
    /// </summary>
    PluginMetadata Metadata { get; }

    /// <summary>
    /// Called exactly once, after the assembly is loaded and before any
    /// InvokeAsync call. Validate configuration and fail fast here rather
    /// than failing on the first real invocation.
    /// </summary>
    Task InitializeAsync(IPluginHostContext hostContext, CancellationToken cancellationToken);

    /// <summary>
    /// May be called concurrently and repeatedly. Must not throw for
    /// expected failure conditions — return a failed PluginInvocationResult
    /// instead; reserve exceptions for genuine bugs (the host will catch
    /// them, but treat that as a safety net, not a control-flow mechanism).
    /// </summary>
    Task<PluginInvocationResult> InvokeAsync(PluginInvocationRequest request, CancellationToken cancellationToken);
}