using System.Linq;
using Pulse.App.Dto.Common;
using Pulse.App.Dto.Connections;
using Pulse.Domain.Aggregates.Connections;

namespace Pulse.App.Handlers.Connections.Common;

public static class DtoMapper
{
    public static ConnectionDto ToDto(this Connection connection)
    {
        return new ConnectionDto
        {
            Id = connection.Id.Value,
            PluginId = connection.PluginId,
            Parameters = connection.Parameters.Select(parameter => new ParameterValueDto
            {
                Key = parameter.Key,
                Value = parameter.Value
            }).ToList()
        };
    }
}