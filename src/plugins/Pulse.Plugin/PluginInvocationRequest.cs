using System.Collections.Generic;
using Pulse.Domain.Channels;

namespace Pulse.Plugin;

public record PluginInvocationRequest
{
    public required List<ParameterValue> InvocationParameters { get; init; }
}