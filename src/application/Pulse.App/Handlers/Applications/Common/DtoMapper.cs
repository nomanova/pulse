using Pulse.App.Dto.Applications;
using Pulse.Domain.Aggregates.Applications;

namespace Pulse.App.Handlers.Applications.Common;

public static class DtoMapper
{
    public static ApplicationDto ToDto(this Application application)
    {
        return new ApplicationDto
        {
            Id = application.Id.Value,
            Name = application.Name.Value,
        };
    }
}