namespace Pulse.Domain.Channels;

public sealed record ParameterDefinition(
    string Key, string Name, string? Description = null, bool IsRequired = true);