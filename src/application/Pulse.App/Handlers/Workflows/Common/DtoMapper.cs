using System.Linq;
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
            Name = workflow.Name.Value,
            DraftVersionId = workflow.DraftVersion?.Id.Value,
            PublishedVersionId = workflow.PublishedVersion?.Id.Value
        };
    }

    public static WorkflowVersionDto ToDto(this WorkflowVersion version)
    {
        return new WorkflowVersionDto
        {
            Id = version.Id.Value,
            WorkflowId = version.WorkflowId.Value,
            Status = (WorkflowVersionStatusDto)version.Status,
            Steps = version.Steps.Select(x => x.ToDto()).ToList()
        };
    }

    public static WorkflowVersionStepDto ToDto(this WorkflowVersionStep step)
    {
        return new WorkflowVersionStepDto
        {
            Id = step.Id.Value,
            Order = step.Order
        };
    }
}