using Pulse.Domain.Common.Models.Entities;
using Pulse.Domain.Common.Services;

namespace Pulse.Domain.Aggregates.Workflows.Entities;

public sealed record WorkflowStepId : EntityId<WorkflowStepId, WorkflowStep>;

public sealed class WorkflowStep : Entity<WorkflowStepId>
{
    public WorkflowId WorkflowId { get; private set; } = null!;

    public uint Order { get; private set; }

    private WorkflowStep()
    {
    }

    private WorkflowStep(WorkflowStepId id, WorkflowId workflowId, uint order) : base(id)
    {
        WorkflowId = workflowId;
        Order = order;
    }

    internal static WorkflowStep Create(Workflow workflow, uint order)
    {
        var id = IdentityProvider.New<WorkflowStepId>();
        return new WorkflowStep(id, workflow.Id, order);
    }
}