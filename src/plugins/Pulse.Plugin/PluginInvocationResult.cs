using System.Collections.Generic;

namespace Pulse.Plugin;

public sealed record PluginInvocationResult(
    bool Success,
    string? Message,
    IReadOnlyDictionary<string, string>? Output
)
{
    public static PluginInvocationResult Ok(IReadOnlyDictionary<string, string>? output = null, string? message = null)
        => new(true, message, output);

    public static PluginInvocationResult Fail(string message)
        => new(false, message, null);
}