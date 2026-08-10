using Pulse.App.Dto.WorkflowInstances;
using Pulse.Domain.Aggregates.WorkflowInstances;

namespace Pulse.App.Handlers.WorkflowInstances.Common;

public static class DtoMapper
{
    public static WorkflowInstanceDto ToDto(this WorkflowInstance workflowInstance)
    {
        return new WorkflowInstanceDto
        {
            Id = workflowInstance.Id.Value,
        };
    }
}