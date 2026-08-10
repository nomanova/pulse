using Pulse.App.Common.Authorization.Requirements;
using Pulse.Domain.Aggregates.Environments;
using Pulse.Domain.Aggregates.WorkflowInstances;
using Pulse.Domain.Aggregates.Workflows;
using Pulse.Domain.Aggregates.Workflows.Entities;

namespace Pulse.App.Common.Authorization.Policies;

public abstract class ApiKeyAuthorizer<TRequest> : Authorizer<TRequest>
{
    public override void BuildPolicy(TRequest request)
    {
        UseRequirement(new MustHaveValidApiKeyRequirement
        {
            EnvironmentId = GetPropertyValue<EnvironmentId>(request, nameof(EnvironmentId)),
            WorkflowId = GetPropertyValue<WorkflowId>(request, nameof(WorkflowId)),
            WorkflowVersionId = GetPropertyValue<WorkflowVersionId>(request, nameof(WorkflowVersionId)),
            WorkflowInstanceId = GetPropertyValue<WorkflowInstanceId>(request, nameof(WorkflowInstanceId))
        });
    }

    private static TValue? GetPropertyValue<TValue>(TRequest request, string propertyName)
        where TValue : class
    {
        return request?
            .GetType()
            .GetProperty(propertyName)?
            .GetValue(request) as TValue;
    }
}