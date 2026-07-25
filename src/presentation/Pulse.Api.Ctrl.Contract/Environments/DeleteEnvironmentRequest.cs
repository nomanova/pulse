namespace Pulse.Api.Ctrl.Contract.Environments;

public sealed record DeleteEnvironmentRequest
{
    public string? OrganizationName { get; init; }
    
    public string? ApplicationName { get; init; }
    
    public string? EnvironmentName { get; init; }
}