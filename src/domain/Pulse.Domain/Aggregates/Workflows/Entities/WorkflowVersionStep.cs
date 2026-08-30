using Pulse.Domain.Aggregates.Workflows.ValueObjects;
using Pulse.Domain.Common.Models.Entities;
using Pulse.Domain.Common.Services;

namespace Pulse.Domain.Aggregates.Workflows.Entities;

public sealed record WorkflowVersionStepId : EntityId<WorkflowVersionStepId, WorkflowVersionStep>;

public sealed class WorkflowVersionStep : Entity<WorkflowVersionStepId>
{
    public WorkflowVersionId WorkflowVersionId { get; private set; } = null!;

    public uint Order { get; private set; }
    
    public IWorkflowStepDefinition Definition { get; private set; } = null!;
    
    private WorkflowVersionStep()
    {
    }

    private WorkflowVersionStep(
        WorkflowVersionStepId id,
        WorkflowVersionId workflowVersionId,
        uint order,
        IWorkflowStepDefinition definition) : base(id)
    {
        WorkflowVersionId = workflowVersionId;
        Order = order;
        Definition = definition;
    }

    internal static WorkflowVersionStep Create(
        WorkflowVersion workflowVersion, 
        uint order,
        IWorkflowStepDefinition definition)
    {
        return new WorkflowVersionStep(
            IdentityProvider.New<WorkflowVersionStepId>(),
            workflowVersion.Id,
            order,
            definition);
    }

    internal static WorkflowVersionStep CreateFrom(
        WorkflowVersion workflowVersion,
        WorkflowVersionStep source)
    {
        return new WorkflowVersionStep(
            IdentityProvider.New<WorkflowVersionStepId>(),
            workflowVersion.Id,
            source.Order,
            source.Definition);
    }

    internal void SetOrder(uint order)
    {
        Order = order;
    }
}