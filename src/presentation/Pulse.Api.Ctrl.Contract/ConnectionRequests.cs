using System.Collections.Generic;
using Pulse.Api.Shared.Contract;

namespace Pulse.Api.Ctrl.Contract;

public sealed record AddConnectionRequest
{
    public string? EnvironmentId { get; init; }
    
    public string? PluginId { get; init; }
    
    public Dictionary<string, string>? Parameters { get; init; }
}

public sealed record RemoveConnectionRequest
{
    public string? ConnectionId { get; init; }
}

public sealed record FetchConnectionRequest
{
    public string? ConnectionId { get; init; }
}

public sealed record SearchConnectionsRequest : PagedSearchRequest
{
    public string? EnvironmentId { get; init; }
}