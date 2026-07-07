using Pulse.Domain.Common.Models.Entities;
using Pulse.Domain.Common.Services;

namespace Pulse.Domain.Aggregates.Workflows.Entities;

public sealed record WorkflowVersionStepId : EntityId<WorkflowVersionStepId, WorkflowVersionStep>;

public sealed class WorkflowVersionStep : Entity<WorkflowVersionStepId>
{
    public WorkflowVersionId WorkflowVersionId { get; private set; } = null!;

    public uint Order { get; private set; }

    private WorkflowVersionStep()
    {
    }

    private WorkflowVersionStep(
        WorkflowVersionStepId id,
        WorkflowVersionId workflowVersionId,
        uint order) : base(id)
    {
        WorkflowVersionId = workflowVersionId;
        Order = order;
    }

    internal static WorkflowVersionStep Create(WorkflowVersion workflowVersion, uint order)
    {
        return new WorkflowVersionStep(
            IdentityProvider.New<WorkflowVersionStepId>(),
            workflowVersion.Id,
            order);
    }

    internal static WorkflowVersionStep CreateFrom(
        WorkflowVersion workflowVersion,
        WorkflowVersionStep source)
    {
        return new WorkflowVersionStep(
            IdentityProvider.New<WorkflowVersionStepId>(),
            workflowVersion.Id,
            source.Order);
    }

    internal void SetOrder(uint order)
    {
        Order = order;
    }
}