using System;
using Pulse.Domain.Aggregates.WorkflowInstances.Enums;
using Pulse.Domain.Aggregates.Workflows.Entities;
using Pulse.Domain.Common.Models.Entities;
using Pulse.Domain.Common.Services;

namespace Pulse.Domain.Aggregates.WorkflowInstances.Entities;

public sealed record WorkflowInstanceStepId : EntityId<WorkflowInstanceStepId, WorkflowInstanceStep>;

public sealed class WorkflowInstanceStep : Entity<WorkflowInstanceStepId>
{
    public WorkflowInstanceId WorkflowInstanceId { get; private set; } = null!;

    public WorkflowVersionStepId WorkflowVersionStepId { get; private set; } = null!;
    
    public uint Order { get; private set; }

    public WorkflowInstanceStepStatus Status { get; private set; }

    public DateTime? StartedAt { get; private set; }

    public DateTime? CompletedAt { get; private set; }

    public DateTime? FailedAt { get; private set; }

    public DateTime? CancelledAt { get; private set; }
    
    private WorkflowInstanceStep()
    {
    }

    private WorkflowInstanceStep(
        WorkflowInstanceStepId id,
        WorkflowInstanceId workflowInstanceId,
        WorkflowVersionStepId workflowVersionStepId,
        uint order,
        WorkflowInstanceStepStatus status) : base(id)
    {
        WorkflowInstanceId = workflowInstanceId;
        WorkflowVersionStepId = workflowVersionStepId;
        Order = order;
        Status = status;
    }
    
    internal static WorkflowInstanceStep Create(
        WorkflowInstance workflowInstance,
        WorkflowVersionStep workflowVersionStep)
    {
        return new WorkflowInstanceStep(
            IdentityProvider.New<WorkflowInstanceStepId>(),
            workflowInstance.Id,
            workflowVersionStep.Id,
            workflowVersionStep.Order,
            WorkflowInstanceStepStatus.Pending);
    }
    
    internal void Start()
    {
        if (Status != WorkflowInstanceStepStatus.Pending)
        {
            throw new InvalidOperationException("Only a pending workflow instance step can be started.");
        }

        Status = WorkflowInstanceStepStatus.Running;
        StartedAt = DateTime.UtcNow;
    }
    
    internal void Complete()
    {
        if (Status != WorkflowInstanceStepStatus.Running)
        {
            throw new InvalidOperationException("Only a running workflow instance step can be completed.");
        }

        Status = WorkflowInstanceStepStatus.Completed;
        CompletedAt = DateTime.UtcNow;
    }

    internal void Skip()
    {
        if (Status != WorkflowInstanceStepStatus.Pending)
        {
            throw new InvalidOperationException("Only a pending workflow instance step can be skipped.");
        }

        Status = WorkflowInstanceStepStatus.Skipped;
        CompletedAt = DateTime.UtcNow;
    }

    internal void Fail()
    {
        if (Status != WorkflowInstanceStepStatus.Running)
        {
            throw new InvalidOperationException("Only a running workflow instance step can fail.");
        }

        Status = WorkflowInstanceStepStatus.Failed;
        FailedAt = DateTime.UtcNow;
    }

    internal void Cancel()
    {
        if (Status is WorkflowInstanceStepStatus.Completed
            or WorkflowInstanceStepStatus.Failed
            or WorkflowInstanceStepStatus.Cancelled)
        {
            return;
        }

        Status = WorkflowInstanceStepStatus.Cancelled;
        CancelledAt = DateTime.UtcNow;
    }
}