namespace Pulse.Api.Ctrl.Contract.Organizations;

public sealed record CreateOrganizationRequest
{
    public string? OrganizationName { get; set; }
}