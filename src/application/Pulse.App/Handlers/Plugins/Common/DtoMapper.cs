using Pulse.App.Dto.Plugins;
using Pulse.Plugin;

namespace Pulse.App.Handlers.Plugins.Common;

public static class DtoMapper
{
    public static PluginDto ToDto(this PluginMetadata metadata)
    {
        return new PluginDto
        {
            Id = metadata.Id,
            DisplayName = metadata.DisplayName,
            Version = metadata.Version,
            Description = metadata.Description
        };
    }
}