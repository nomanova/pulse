using Pulse.Api.Shared.Contract;

namespace Pulse.Api.Ctrl.Contract;

public sealed record AddApplicationRequest
{
    public string? OrganizationId { get; init; }

    public string? ApplicationName { get; init; }
}

public sealed record RemoveApplicationRequest
{
    public string? ApplicationId { get; init; }
}

public sealed record FetchApplicationRequest
{
    public string? ApplicationId { get; init; }
}

public sealed record SearchApplicationsRequest : NamedPagedSearchRequest
{
    public string? OrganizationId { get; init; }
}