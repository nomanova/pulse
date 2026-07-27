namespace Pulse.Api.Ctrl.Contract.Workflows;

public sealed record DeleteWorkflowRequest
{
    public string? OrganizationName { get; init; }
    
    public string? ApplicationName { get; init; }
    
    public string? EnvironmentName { get; init; }
    
    public string? WorkflowName { get; init; }
}