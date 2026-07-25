using Pulse.App.Dto.Environments;
using Pulse.Domain.Aggregates.Environments;

namespace Pulse.App.Handlers.Environments.Common;

public static class DtoMapper
{
    public static EnvironmentDto ToDto(this Environment environment)
    {
        return new EnvironmentDto
        {
            Id = environment.Id.Value,
            Name = environment.Name.Value,
        };
    }
}