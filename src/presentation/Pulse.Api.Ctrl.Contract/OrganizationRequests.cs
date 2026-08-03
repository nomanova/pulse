namespace Pulse.Api.Ctrl.Contract;

public abstract record OrganizationRequest
{
    public string? OrganizationId { get; init; }
}

public sealed record AddOrganizationRequest
{
    public string? OrganizationName { get; init; }
}

public sealed record RemoveOrganizationRequest : OrganizationRequest;

public sealed record FetchOrganizationRequest : OrganizationRequest;