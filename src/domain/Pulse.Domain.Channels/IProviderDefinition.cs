using System.Collections.Generic;

namespace Pulse.Domain.Channels;

public interface IProviderDefinition
{
    Channel Channel { get; }

    List<ParameterDefinition> InvocationParameters { get; }

    ParameterValidationResult CanInvoke(List<ParameterValue> invocationValues);
}