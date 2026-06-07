namespace Pulse.Api.Mgmt.Contract.Applications;

public sealed record CreateApplicationRequest
{
    public string? OrganizationId { get; set; }
    
    public string? Name { get; set; }
}