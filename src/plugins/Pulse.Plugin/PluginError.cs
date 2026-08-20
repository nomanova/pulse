namespace Pulse.Plugin;

public sealed record PluginError(string Message, string? ParameterKey = null);
