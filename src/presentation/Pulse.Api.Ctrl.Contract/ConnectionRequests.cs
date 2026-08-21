using System.Collections.Generic;

namespace Pulse.Api.Ctrl.Contract;

public sealed record AddConnectionRequest
{
    public string? EnvironmentId { get; init; }
    
    public string? PluginId { get; init; }
    
    public Dictionary<string, string>? Parameters { get; init; }
}