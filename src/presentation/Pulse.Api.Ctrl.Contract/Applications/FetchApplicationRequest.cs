namespace Pulse.Api.Ctrl.Contract.Applications;

public sealed record FetchApplicationRequest
{
    public string? OrganizationName { get; set; }
    
    public string? ApplicationName { get; set; }
}