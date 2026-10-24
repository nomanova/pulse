using Pulse.App.Dto.Common;

namespace Pulse.App.Dto.Applications;

public sealed record ApplicationDto : IdentityDto
{
    public required string Name { get; init; }
    
    public required string Key { get; init; }
}