namespace Pulse.App.Dto.Workflows;

public sealed record WorkflowVersionStepDto
{
    public required string Id { get; init; }
    
    public required uint Order { get; init; }
}