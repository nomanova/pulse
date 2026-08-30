using System;
using Pulse.Domain.Aggregates.WorkflowInstances.Enums;
using Pulse.Domain.Aggregates.Workflows.Entities;
using Pulse.Domain.Aggregates.Workflows.ValueObjects;
using Pulse.Domain.Common.Models.Entities;
using Pulse.Domain.Common.Services;

namespace Pulse.Domain.Aggregates.WorkflowInstances.Entities;

public sealed record WorkflowInstanceStepId : EntityId<WorkflowInstanceStepId, WorkflowInstanceStep>;

public sealed class WorkflowInstanceStep : Entity<WorkflowInstanceStepId>
{
    public WorkflowInstanceId WorkflowInstanceId { get; private set; } = null!;

    public WorkflowVersionStepId? WorkflowVersionStepId { get; private set; }
    
    public uint Order { get; private set; }

    public IWorkflowStepDefinition Definition { get; private set; } = null!;

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
        WorkflowVersionStepId? workflowVersionStepId,
        uint order,
        IWorkflowStepDefinition definition,
        WorkflowInstanceStepStatus status) : base(id)
    {
        WorkflowInstanceId = workflowInstanceId;
        WorkflowVersionStepId = workflowVersionStepId;
        Order = order;
        Definition = definition;
        Status = status;
    }

    internal static WorkflowInstanceStep Create(
        WorkflowInstance workflowInstance,
        uint order,
        IWorkflowStepDefinition definition,
        WorkflowVersionStep? workflowVersionStep = null)
    {
        var id = IdentityProvider.New<WorkflowInstanceStepId>();

        return new WorkflowInstanceStep(
            id, 
            workflowInstance.Id, 
            workflowVersionStep?.Id, 
            order, 
            definition,
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