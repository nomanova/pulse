namespace Pulse.Api.Ctrl.Contract.Environments;

public sealed record FetchEnvironmentRequest
{
    public string? OrganizationName { get; set; }
    
    public string? ApplicationName { get; set; }
    
    public string? EnvironmentName { get; set; }
}