using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using Pulse.App.Common.Security.Interfaces;
using Pulse.Domain.Aggregates.Environments;
using Pulse.Domain.Aggregates.WorkflowInstances;
using Pulse.Domain.Aggregates.Workflows;

namespace Pulse.App.Common.Authorization.Requirements;

public class MustHaveValidApiKeyRequirement : IAuthorizationRequirement
{
    public EnvironmentId? EnvironmentId { get; init; }
    
    public WorkflowId? WorkflowId { get; init; }

    public WorkflowInstanceId? WorkflowInstanceId { get; init; }
}

public class MustHaveValidApiKeyRequirementHandler : IAuthorizationHandler<MustHaveValidApiKeyRequirement>
{
    private readonly IUserClaimProvider _userClaimProvider;
    private readonly IApiKeyAuthorizationReader _apiKeyAuthorizationReader;

    public MustHaveValidApiKeyRequirementHandler(
        IUserClaimProvider userClaimProvider,
        IApiKeyAuthorizationReader apiKeyAuthorizationReader)
    {
        _userClaimProvider = userClaimProvider;
        _apiKeyAuthorizationReader = apiKeyAuthorizationReader;
    }

    public async Task<ErrorOr<Success>> Handle(
        MustHaveValidApiKeyRequirement request,
        CancellationToken cancellationToken)
    {
        var apiKey = _userClaimProvider.ApiKey;
        
        var isValid = request switch
        {
            { EnvironmentId: not null } => await _apiKeyAuthorizationReader.HasValidApiKeyForEnvironment(
                request.EnvironmentId,
                apiKey,
                cancellationToken),
            
            { WorkflowId: not null } => await _apiKeyAuthorizationReader.HasValidApiKeyForWorkflow(
                request.WorkflowId,
                apiKey,
                cancellationToken),

            { WorkflowInstanceId: not null } => await _apiKeyAuthorizationReader.HasValidApiKeyForWorkflowInstance(
                request.WorkflowInstanceId,
                apiKey,
                cancellationToken),

            _ => false
        };

        return isValid
            ? Result.Success
            : AuthorizationErrors.InvalidApiKey;
    }
}