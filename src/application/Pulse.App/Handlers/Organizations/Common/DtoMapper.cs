using Pulse.App.Dto.Organizations;
using Pulse.Domain.Aggregates.Organizations;

namespace Pulse.App.Handlers.Organizations.Common;

public static class DtoMapper
{
    public static OrganizationDto ToDto(this Organization organization)
    {
        return new OrganizationDto
        {
            Id = organization.Id.Value,
            Name = organization.Name.Value
        };
    }
}