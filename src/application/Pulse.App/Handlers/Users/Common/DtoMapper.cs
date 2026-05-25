using Pulse.App.Dto.Users;
using Pulse.Domain.Aggregates.Users;

namespace Pulse.App.Handlers.Users.Common;

public static class DtoMapper
{
    public static UserDto ToDto(this User entity)
    {
        return new UserDto
        {
            Id = entity.Id.Value,
            Username = entity.Username.Value
        };
    }
}