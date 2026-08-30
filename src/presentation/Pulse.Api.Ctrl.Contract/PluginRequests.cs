using System.Collections.Generic;

namespace Pulse.Api.Ctrl.Contract;

public sealed record FetchPluginRequest
{
    public string? PluginId { get; init; }
}

public sealed record VerifyConnectPluginRequest
{
    public string? PluginId { get; init; }
    
    public Dictionary<string, string>? Parameters { get; init; }
}

public sealed record VerifyInvokePluginRequest
{
    public string? PluginId { get; init; }
    
    public Dictionary<string, string>? Parameters { get; init; }
}