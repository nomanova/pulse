using Pulse.App.Dto.Common;

namespace Pulse.App.Dto.Plugins;

public sealed record PluginDto
{
    public required string Id { get; init; }

    public required string DisplayName { get; init; }

    public required string Version { get; init; }
    
    public required string Description { get; init; }
    
    public PluginTypeDto? Type { get; set; }
    
    public ChannelDto? Channel { get; set; }
    
    public List<ParameterDefinitionDto>? InvocationParameters { get; set; }
    
    public List<ParameterDefinitionDto>? ConnectionParameters { get; set; }
}