using System.Collections.Generic;
using Pulse.Domain.Channels;

namespace Pulse.Plugin.Providers;

public sealed record ProviderPluginInvocationRequest : PluginInvocationRequest
{
    public required List<ParameterValue> ConnectionParameters { get; init; }
}