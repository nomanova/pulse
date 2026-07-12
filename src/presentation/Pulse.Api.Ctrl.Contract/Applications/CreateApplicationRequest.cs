namespace Pulse.Api.Ctrl.Contract.Applications;

public sealed record CreateApplicationRequest
{
    public string? OrganizationId { get; set; }
    
    public string? Name { get; set; }
}