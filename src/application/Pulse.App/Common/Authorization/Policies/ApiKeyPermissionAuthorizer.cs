using Pulse.App.Common.Authorization.Requirements;
using Pulse.Domain.Aggregates.Workflows;

namespace Pulse.App.Common.Authorization.Policies;

public abstract class ApiKeyPermissionAuthorizer<TRequest> : Authorizer<TRequest>
{
    public override void BuildPolicy(TRequest request)
    {
        UseRequirement(new MustHaveApiKeyResourcePermissionRequirement
        {
            WorkflowId = GetPropertyValue<WorkflowId>(request, nameof(WorkflowId))
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