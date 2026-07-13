namespace Pulse.Api.Ctrl.Contract.Applications;

public sealed record CreateApplicationRequest
{
    public string? OrganizationName { get; set; }
    
    public string? ApplicationName { get; set; }
}