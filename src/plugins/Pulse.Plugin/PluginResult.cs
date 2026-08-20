using System.Collections.Generic;

namespace Pulse.Plugin;

public sealed record PluginResult
{
    public static PluginResult Success()
    {
        return new PluginResult { IsSuccess = true };
    }

    public static PluginResult Failure(List<PluginError> errors)
    {
        return new PluginResult { IsSuccess = false, Errors = errors };
    }
    
    public static PluginResult Failure(params PluginError[] errors)
    {
        return new PluginResult { IsSuccess = false, Errors = [..errors] };
    }

    public bool IsSuccess { get; private init; }

    public List<PluginError> Errors { get; private init; } = [];
}