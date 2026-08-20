using System.Collections.Generic;

namespace Pulse.Plugin;

public record PluginInvocationRequest
{
    public required List<PluginParameterValue> InvocationParameters { get; init; }
}