using Pulse.App.Dto.Workflows;
using Pulse.Domain.Aggregates.Workflows;

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
}