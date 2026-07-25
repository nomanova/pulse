using Pulse.App.Dto.Common;

namespace Pulse.App.Dto.Environments;

public sealed record EnvironmentDto : IdentityDto
{
    public required string Name { get; init; }
}