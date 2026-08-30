using System.Collections.Generic;
using Pulse.Domain.Channels;

namespace Pulse.Plugin;

public class PluginValidationResult
{
    public List<ParameterValidationError> Errors { get; private init; } = [];
}