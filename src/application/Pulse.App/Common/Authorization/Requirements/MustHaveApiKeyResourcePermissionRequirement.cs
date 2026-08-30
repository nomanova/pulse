using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using Pulse.App.Common.Security.Interfaces;
using Pulse.Domain.Aggregates.WorkflowInstances;
using Pulse.Domain.Aggregates.Workflows;
using Pulse.Domain.Aggregates.Workflows.Entities;

namespace Pulse.App.Common.Authorization.Requirements;

public class MustHaveApiKeyResourcePermissionRequirement : IAuthorizationRequirement
{
    public WorkflowId? WorkflowId { get; init; }

    public WorkflowVersionId? WorkflowVersionId { get; init; }
    
    public WorkflowInstanceId? WorkflowInstanceId { get; init; }
}

public class MustHaveApiKeyResourcePermissionRequirementHandler : 
    IAuthorizationHandler<MustHaveApiKeyResourcePermissionRequirement>
{
    private readonly IEnvironmentProvider _environmentProvider;
    private readonly IApiKeyAuthorizationReader _apiKeyAuthorizationReader;

    public MustHaveApiKeyResourcePermissionRequirementHandler(
        IEnvironmentProvider environmentProvider, 
        IApiKeyAuthorizationReader apiKeyAuthorizationReader)
    {
        _environmentProvider = environmentProvider;
        _apiKeyAuthorizationReader = apiKeyAuthorizationReader;
    }

    public async Task<ErrorOr<Success>> Handle(
        MustHaveApiKeyResourcePermissionRequirement request, 
        CancellationToken cancellationToken)
    {
        var environment = await _environmentProvider.Get(cancellationToken);
        
        var isValid = request switch
        {
            { WorkflowId: not null } => await _apiKeyAuthorizationReader.BelongsToEnvironment(
                request.WorkflowId,
                environment.Id,
                cancellationToken),

            { WorkflowVersionId: not null } => await _apiKeyAuthorizationReader.BelongsToEnvironment(
                request.WorkflowVersionId,
                environment.Id,
                cancellationToken),
            
            { WorkflowInstanceId: not null } => await _apiKeyAuthorizationReader.BelongsToEnvironment(
                request.WorkflowInstanceId,
                environment.Id,
                cancellationToken),

            _ => false
        };
        
        return isValid
            ? Result.Success
            : AuthorizationErrors.InvalidApiKey;
    }
}