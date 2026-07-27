using Pulse.App.Dto.Common;

namespace Pulse.App.Dto.Workflows;

public sealed record WorkflowDto : IdentityDto
{
    public required string Name { get; init; }
}