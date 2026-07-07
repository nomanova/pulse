using System.Collections.Generic;
using System.Linq;
using Pulse.Domain.Aggregates.Workflows.Enums;
using Pulse.Domain.Common.Errors;
using Pulse.Domain.Common.Models.Entities;
using Pulse.Domain.Common.Services;

namespace Pulse.Domain.Aggregates.Workflows.Entities;

public sealed record WorkflowVersionId : EntityId<WorkflowVersionId, WorkflowVersion>;

public sealed class WorkflowVersion : Entity<WorkflowVersionId>
{
    public WorkflowId WorkflowId { get; private set; } = null!;

    public uint Version { get; private set; }

    public WorkflowVersionStatus Status { get; private set; }

    private readonly List<WorkflowVersionStep> _steps = [];

    public IReadOnlyCollection<WorkflowVersionStep> Steps => _steps
        .OrderBy(step => step.Order)
        .ToList()
        .AsReadOnly();

    public bool IsDraft => Status == WorkflowVersionStatus.Draft;

    public bool IsPublished => Status == WorkflowVersionStatus.Published;

    private WorkflowVersion()
    {
    }

    private WorkflowVersion(
        WorkflowVersionId id,
        WorkflowId workflowId,
        uint version,
        WorkflowVersionStatus status) : base(id)
    {
        WorkflowId = workflowId;
        Version = version;
        Status = status;
    }

    internal static WorkflowVersion CreateDraft(Workflow workflow, uint version)
    {
        return new WorkflowVersion(
            IdentityProvider.New<WorkflowVersionId>(),
            workflow.Id,
            version,
            WorkflowVersionStatus.Draft);
    }

    internal static WorkflowVersion CreateDraftFrom(
        Workflow workflow,
        uint version,
        WorkflowVersion source)
    {
        var draft = CreateDraft(workflow, version);

        foreach (var sourceStep in source.Steps)
        {
            draft._steps.Add(WorkflowVersionStep.CreateFrom(draft, sourceStep));
        }

        return draft;
    }

    public WorkflowVersionStep AddStep()
    {
        EnsureDraft();

        var order = _steps.Count == 0
            ? 1
            : _steps.Max(step => step.Order) + 1;

        var step = WorkflowVersionStep.Create(this, order);

        _steps.Add(step);

        return step;
    }

    public void RemoveStep(WorkflowVersionStepId stepId)
    {
        EnsureDraft();

        var step = _steps.SingleOrDefault(candidate => candidate.Id == stepId);

        DomainErrors.Workflow.UnknownStep.Assert(() => step != null);

        _steps.Remove(step!);

        ReorderSteps();
    }

    public void MoveStep(WorkflowVersionStepId stepId, uint order)
    {
        EnsureDraft();

        DomainErrors.Workflow.InvalidOrder.Assert(() =>
            !(order == 0 || order > _steps.Count));

        var step = _steps.SingleOrDefault(candidate => candidate.Id == stepId);

        DomainErrors.Workflow.UnknownStep.Assert(() => step != null);

        _steps.Remove(step!);
        _steps.Insert((int)order - 1, step!);

        ReorderSteps();
    }

    internal void Publish()
    {
        EnsureDraft();

        DomainErrors.Workflow.NoSteps.Assert(() => _steps.Any());

        Status = WorkflowVersionStatus.Published;
    }

    internal void Archive()
    {
        DomainErrors.Workflow.VersionNotPublished.Assert(() => IsPublished);

        Status = WorkflowVersionStatus.Archived;
    }

    private void EnsureDraft()
    {
        DomainErrors.Workflow.VersionNotDraft.Assert(() => IsDraft);
    }

    private void ReorderSteps()
    {
        var orderedSteps = _steps
            .OrderBy(step => step.Order)
            .ToList();

        _steps.Clear();

        for (var index = 0; index < orderedSteps.Count; index++)
        {
            var step = orderedSteps[index];
            step.SetOrder((uint)index + 1);
            _steps.Add(step);
        }
    }
}