using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using ErrorOr;
using Pulse.Domain.Aggregates.Workflows.Enums;
using Pulse.Domain.Channels;

namespace Pulse.Domain.Aggregates.Workflows.ValueObjects;

public interface IWorkflowStepDefinition
{
    WorkflowStepDefinitionType Type { get; }
}

public sealed record ProviderWorkflowStepDefinition : IWorkflowStepDefinition
{
    public WorkflowStepDefinitionType Type => WorkflowStepDefinitionType.Provider;
    
    [JsonInclude]
    public Channel Channel { get; private set; }

    [JsonInclude]
    public IReadOnlyList<ParameterValue> InvocationValues { get; private set; } = [];

    private ProviderWorkflowStepDefinition()
    {
    }

    private ProviderWorkflowStepDefinition(
        Channel channel,
        List<ParameterValue> invocationValues)
    {
        Channel = channel;
        InvocationValues = invocationValues;
    }

    public static ErrorOr<ProviderWorkflowStepDefinition> ForEmail(
        List<ParameterValue> invocationValues)
    {
        var result = EmailProviderDefinition.Instance.CanInvoke(invocationValues);

        if (!result.IsSuccess)
        {
            return result.Errors.ToList().ConvertAll(error =>
                Error.Validation(error.ParameterKey, error.Message));
        }

        return new ProviderWorkflowStepDefinition(EmailProviderDefinition.Instance.Channel, invocationValues);
    }
}