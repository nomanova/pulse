using Pulse.Api.Shared.Contract;

namespace Pulse.Api.Ctrl.Contract;

public sealed record AddEnvironmentRequest
{
    public string? ApplicationId { get; init; }
    
    public string? EnvironmentName { get; init; }
}

public sealed record RemoveEnvironmentRequest
{
    public string? EnvironmentId { get; init; }
}

public sealed record FetchEnvironmentRequest
{
    public string? EnvironmentId { get; init; }
}

public sealed record SearchEnvironmentsRequest : NamedPagedSearchRequest
{
    public string? ApplicationId { get; init; }
}