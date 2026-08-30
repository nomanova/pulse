using System.Collections.Generic;
using System.Linq;
using Pulse.App.Dto.Common;
using Pulse.App.Dto.Plugins;
using Pulse.Domain.Channels;
using Pulse.Plugin;
using Pulse.Plugin.Providers;

namespace Pulse.App.Handlers.Plugins.Common;

public static class DtoMapper
{
    public static PluginDto ToDto(this IPlugin plugin)
    {
        var metadata = plugin.Metadata;

        var dto = new PluginDto
        {
            Id = metadata.Id,
            DisplayName = metadata.DisplayName,
            Version = metadata.Version,
            Description = metadata.Description
        };

        if (plugin is IProviderPlugin providerPlugin)
        {
            dto.Type = PluginTypeDto.Provider;
            dto.Channel = (ChannelDto)providerPlugin.Channel;

            dto.ConnectionParameters = providerPlugin.ConnectionParameters.ToDto();
            dto.InvocationParameters = providerPlugin.InvocationParameters.ToDto();
        }

        return dto;
    }

    private static List<PluginParameterDefinitionDto> ToDto(this IEnumerable<ParameterDefinition> parameters)
    {
        return parameters.Select(p => new PluginParameterDefinitionDto
        {
            Key = p.Key,
            Name = p.Name,
            Description = p.Description,
            IsRequired = p.IsRequired
        }).ToList();
    }
}