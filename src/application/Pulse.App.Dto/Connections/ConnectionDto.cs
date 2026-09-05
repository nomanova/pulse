using Pulse.App.Dto.Common;

namespace Pulse.App.Dto.Connections;

public sealed record ConnectionDto : IdentityDto
{
    public ChannelDto Channel { get; init; }
    
    public required string PluginId { get; init; }
    
    public IReadOnlyList<ParameterValueDto> Parameters { get; init; } = [];
}