using Pulse.App.Dto.Common;
using Pulse.Domain.Common.Models.Entities;

namespace Pulse.App.Common.Mappers;

public static class IdentityDtoMapper
{
    public static IdentityDto ToIdentityDto<T>(this Entity<T> entity) where T : EntityId
    {
        return new IdentityDto
        {
            Id = entity.Id.Value
        };
    }
}