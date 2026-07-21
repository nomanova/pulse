namespace Pulse.Api.Ctrl.Contract.Organizations;

public sealed record DeleteOrganizationRequest
{
    public string? OrganizationName { get; set; }
}