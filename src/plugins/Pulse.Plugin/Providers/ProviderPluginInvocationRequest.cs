using System.Collections.Generic;

namespace Pulse.Plugin.Providers;

public sealed record ProviderPluginInvocationRequest : PluginInvocationRequest
{
    public required List<PluginParameterValue> ConnectionParameters { get; init; }
}