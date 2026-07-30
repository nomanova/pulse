using Pulse.App.Dto.Workflows;
using Pulse.Domain.Aggregates.Workflows;
using Pulse.Domain.Aggregates.Workflows.Entities;

namespace Pulse.App.Handlers.Workflows.Common;

public static class DtoMapper
{
    public static WorkflowDto ToDto(this Workflow workflow)
    {
        return new WorkflowDto
        {
            Id = workflow.Id.Value,
            Name = workflow.Name.Value
        };
    }
    
    public static WorkflowVersionStepDto ToDto(this WorkflowVersionStep step)
    {
        return new WorkflowVersionStepDto
        {
            Id = step.Id.Value,
            WorkflowVersionId = step.WorkflowVersionId.Value,
            Order = step.Order
        };
    }
}