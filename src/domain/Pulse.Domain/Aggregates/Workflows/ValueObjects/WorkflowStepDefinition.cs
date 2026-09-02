using System.Collections.Generic;
using System.Text.Json.Serialization;
using ErrorOr;
using Pulse.Domain.Aggregates.Workflows.Enums;
using Pulse.Domain.Channels;
using Pulse.Domain.Channels.Definitions;
using Pulse.Domain.Common.Errors;

namespace Pulse.Domain.Aggregates.Workflows.ValueObjects;

public interface IWorkflowStepDefinition
{
    WorkflowStepDefinitionType Type { get; }
}

public sealed record ProviderWorkflowStepDefinition : IWorkflowStepDefinition
{
    public WorkflowStepDefinitionType Type => WorkflowStepDefinitionType.Provider;

    [JsonInclude] public Channel Channel { get; private set; }

    [JsonInclude] public IReadOnlyList<ParameterValue> Parameters { get; private set; } = [];

    [JsonConstructor]
    private ProviderWorkflowStepDefinition()
    {
    }

    private ProviderWorkflowStepDefinition(
        Channel channel,
        List<ParameterValue> parameters)
    {
        Channel = channel;
        Parameters = parameters;
    }

    public static ErrorOr<ProviderWorkflowStepDefinition> ForEmail(List<ParameterValue> parameters)
    {
        var result = EmailProviderDefinition.Instance.CanInvoke(parameters);

        if (!result.IsSuccess)
        {
            return result.Errors.Map();
        }

        return new ProviderWorkflowStepDefinition(EmailProviderDefinition.Instance.Channel, parameters);
    }

    public static ErrorOr<ProviderWorkflowStepDefinition> ForSms(List<ParameterValue> parameters)
    {
        var result = SmsProviderDefinition.Instance.CanInvoke(parameters);

        if (!result.IsSuccess)
        {
            return result.Errors.Map();
        }

        return new ProviderWorkflowStepDefinition(SmsProviderDefinition.Instance.Channel, parameters);
    }
}