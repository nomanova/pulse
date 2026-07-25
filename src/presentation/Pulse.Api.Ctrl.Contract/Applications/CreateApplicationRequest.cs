namespace Pulse.Api.Ctrl.Contract.Applications;

public sealed record CreateApplicationRequest
{
    public string? OrganizationName { get; init; }
    
    public string? ApplicationName { get; init; }
}