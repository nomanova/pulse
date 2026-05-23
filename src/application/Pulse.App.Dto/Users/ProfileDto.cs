using Pulse.App.Dto.Organizations;

namespace Pulse.App.Dto.Users;

public sealed record ProfileDto
{
    public required UserDto User { get; init; }
    
    public OrganizationDto? Organization { get; init; }
}