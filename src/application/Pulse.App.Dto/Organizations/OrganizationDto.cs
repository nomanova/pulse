using Pulse.App.Dto.Common;

namespace Pulse.App.Dto.Organizations;

public sealed record OrganizationDto : IdentityDto
{
    public required string Name { get; init; }
}