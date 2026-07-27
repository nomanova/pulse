namespace Pulse.Api.Ctrl.Contract.Workflows;

public sealed record FetchWorkflowRequest
{    
    public string? OrganizationName { get; set; }
    
    public string? ApplicationName { get; set; }
    
    public string? EnvironmentName { get; set; }
    
    public string? WorkflowName { get; set; }
}