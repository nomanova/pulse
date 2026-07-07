using System;
using System.Collections.Generic;
using System.Linq;
using Pulse.Domain.Aggregates.Environments;
using Pulse.Domain.Aggregates.Organizations;
using Pulse.Domain.Aggregates.WorkflowInstances.Entities;
using Pulse.Domain.Aggregates.WorkflowInstances.Enums;
using Pulse.Domain.Aggregates.WorkflowInstances.Events;
using Pulse.Domain.Aggregates.Workflows;
using Pulse.Domain.Aggregates.Workflows.Entities;
using Pulse.Domain.Common.Models.Entities;
using Pulse.Domain.Common.Services;
using ApplicationId = Pulse.Domain.Aggregates.Applications.ApplicationId;

namespace Pulse.Domain.Aggregates.WorkflowInstances;

public sealed record WorkflowInstanceId : EntityId<WorkflowInstanceId, WorkflowInstance>;

public sealed class WorkflowInstance : DomainEntity<WorkflowInstanceId>, IEnvironmentScoped
{
    public OrganizationId OrganizationId { get; private set; } = null!;

    public ApplicationId ApplicationId { get; private set; } = null!;

    public EnvironmentId EnvironmentId { get; private set; } = null!;

    public WorkflowId WorkflowId { get; private set; } = null!;

    public WorkflowVersionId WorkflowVersionId { get; private set; } = null!;

    public WorkflowInstanceStatus Status { get; private set; }

    private readonly List<WorkflowInstanceStep> _steps = [];

    public IReadOnlyCollection<WorkflowInstanceStep> Steps => _steps
        .OrderBy(step => step.Order)
        .ToList()
        .AsReadOnly();

    public WorkflowInstanceStep? CurrentStep => _steps
        .Where(step => step.Status == WorkflowInstanceStepStatus.Running)
        .OrderBy(step => step.Order)
        .SingleOrDefault();

    private WorkflowInstance()
    {
    }

    private WorkflowInstance(
        WorkflowInstanceId id,
        OrganizationId organizationId,
        ApplicationId applicationId,
        EnvironmentId environmentId,
        WorkflowId workflowId,
        WorkflowVersionId workflowVersionId,
        WorkflowInstanceStatus status) : base(id)
    {
        OrganizationId = organizationId;
        ApplicationId = applicationId;
        EnvironmentId = environmentId;
        WorkflowId = workflowId;
        WorkflowVersionId = workflowVersionId;
        Status = status;
    }

    internal static WorkflowInstance Create(
        Workflow workflow,
        WorkflowVersion workflowVersion)
    {
        var instance = new WorkflowInstance(
            IdentityProvider.New<WorkflowInstanceId>(),
            workflow.OrganizationId,
            workflow.ApplicationId,
            workflow.EnvironmentId,
            workflow.Id,
            workflowVersion.Id,
            WorkflowInstanceStatus.Running);

        foreach (var workflowVersionStep in workflowVersion.Steps)
        {
            instance._steps.Add(WorkflowInstanceStep.Create(instance, workflowVersionStep));
        }

        instance.StartNextPendingStep();

        instance.SetCreated();

        return instance;
    }

    public void CompleteStep(WorkflowInstanceStepId stepId)
    {
        var step = GetStep(stepId);

        if (step.Status == WorkflowInstanceStepStatus.Completed)
        {
            return;
        }

        EnsureRunning();

        step.Complete();

        if (HasPendingSteps())
        {
            StartNextPendingStep();
        }
        else
        {
            Complete();
        }

        SetModified();
    }

    public void FailStep(WorkflowInstanceStepId stepId)
    {
        EnsureRunning();

        var step = GetStep(stepId);

        step.Fail();

        Status = WorkflowInstanceStatus.Failed;

        SetModified();
    }

    public void CompleteCurrentStep()
    {
        var currentStep = GetCurrentStep();

        CompleteStep(currentStep.Id);
    }

    public void FailCurrentStep()
    {
        var currentStep = GetCurrentStep();

        FailStep(currentStep.Id);
    }

    public void Cancel()
    {
        if (Status is WorkflowInstanceStatus.Completed
            or WorkflowInstanceStatus.Cancelled
            or WorkflowInstanceStatus.Failed)
        {
            return;
        }

        foreach (var step in _steps)
        {
            step.Cancel();
        }

        Status = WorkflowInstanceStatus.Cancelled;

        SetModified();
    }

    private WorkflowInstanceStep GetStep(WorkflowInstanceStepId stepId)
    {
        return _steps.SingleOrDefault(step => step.Id == stepId)
               ?? throw new InvalidOperationException("The workflow instance step does not belong to this workflow instance.");
    }
    
    private void Complete()
    {
        Status = WorkflowInstanceStatus.Completed;
    }

    private void StartNextPendingStep()
    {
        var nextStep = _steps
            .Where(step => step.Status == WorkflowInstanceStepStatus.Pending)
            .OrderBy(step => step.Order)
            .FirstOrDefault();

        if (nextStep is null)
        {
            return;
        }

        nextStep.Start();
        
        AddEvent(new WorkflowInstanceStepStartedEvent(
            Id,
            nextStep.Id,
            WorkflowId,
            WorkflowVersionId,
            nextStep.WorkflowVersionStepId,
            nextStep.Order));
    }

    private WorkflowInstanceStep GetCurrentStep()
    {
        return CurrentStep
               ?? throw new InvalidOperationException("The workflow instance does not have a running step.");
    }

    private bool HasPendingSteps()
    {
        return _steps.Any(step => step.Status == WorkflowInstanceStepStatus.Pending);
    }

    private void EnsureRunning()
    {
        if (Status != WorkflowInstanceStatus.Running)
        {
            throw new InvalidOperationException("The workflow instance is not running.");
        }
    }
}