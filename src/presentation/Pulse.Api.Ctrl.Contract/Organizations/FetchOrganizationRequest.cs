namespace Pulse.Api.Ctrl.Contract.Organizations;

public sealed record FetchOrganizationRequest
{
    public string? OrganizationName { get; set; }
}