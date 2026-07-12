namespace Pulse.Api.Ctrl.Contract.Environments;

public sealed record CreateEnvironmentRequest
{
    public string? OrganizationId { get; set; }
    
    public string? ApplicationId { get; set; }
    
    public string? Name { get; set; }
}