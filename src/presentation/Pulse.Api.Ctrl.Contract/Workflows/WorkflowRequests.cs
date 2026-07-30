using Pulse.Api.Shared.Contract;

namespace Pulse.Api.Ctrl.Contract.Workflows;

public abstract record WorkflowRequest
{
    public string? OrganizationName { get; init; }
    
    public string? ApplicationName { get; init; }
    
    public string? EnvironmentName { get; init; } 
    
    public string? WorkflowName { get; init; } 
}

public sealed record CreateWorkflowRequest : WorkflowRequest;

public sealed record DeleteWorkflowRequest : WorkflowRequest;

public sealed record FetchWorkflowRequest : WorkflowRequest;

public sealed record SearchWorkflowsRequest : PagedSearchRequest
{
    public string? OrganizationName { get; init; }
    
    public string? ApplicationName { get; init; }
    
    public string? EnvironmentName { get; init; }
}

public sealed record AddWorkflowStepRequest : WorkflowRequest;