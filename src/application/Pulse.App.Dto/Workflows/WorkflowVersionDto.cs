using Pulse.App.Dto.Common;

namespace Pulse.App.Dto.Workflows;

public sealed record WorkflowVersionDto : IdentityDto
{
    public required string WorkflowId { get; init; }

    public IReadOnlyCollection<WorkflowVersionStepDto> Steps { get; init; } = [];
}