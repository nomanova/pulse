namespace Pulse.App.Dto.Plugins;

public sealed record PluginDto
{
    public required string Id { get; init; }

    public required string DisplayName { get; init; }

    public required string Version { get; init; }
    
    public required string Description { get; init; }
}