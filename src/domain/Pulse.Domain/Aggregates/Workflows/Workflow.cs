using System.Collections.Generic;
using System.Linq;
using Pulse.Domain.Aggregates.Environments;
using Pulse.Domain.Aggregates.WorkflowInstances;
using Pulse.Domain.Aggregates.Workflows.Entities;
using Pulse.Domain.Common.Errors;
using Pulse.Domain.Common.Models.Entities;
using Pulse.Domain.Common.Models.ValueObjects;
using Pulse.Domain.Common.Services;
using Environment = Pulse.Domain.Aggregates.Environments.Environment;

namespace Pulse.Domain.Aggregates.Workflows;

public sealed record WorkflowId : EntityId<WorkflowId, Workflow>;

public sealed class Workflow : DomainEntity<WorkflowId>, IEnvironmentScoped, INamedObject, IKeyedObject
{
    public EnvironmentId EnvironmentId { get; private set; } = null!;

    public ObjectName Name { get; private set; } = null!;
    
    public ObjectKey Key { get; private set; } = null!;

    public WorkflowVersionId? PublishedVersionId { get; private set; }

    public WorkflowVersionId? DraftVersionId { get; private set; }

    private readonly List<WorkflowVersion> _versions = [];

    public IReadOnlyCollection<WorkflowVersion> Versions => _versions
        .OrderBy(version => version.Version)
        .ToList()
        .AsReadOnly();

    public WorkflowVersion? PublishedVersion => PublishedVersionId is null
        ? null
        : _versions.Single(version => version.Id == PublishedVersionId);

    public WorkflowVersion? DraftVersion => DraftVersionId is null
        ? null
        : _versions.Single(version => version.Id == DraftVersionId);

    private Workflow()
    {
    }

    private Workflow(
        WorkflowId id,
        EnvironmentId environmentId,
        ObjectName name,
        ObjectKey key) : base(id)
    {
        EnvironmentId = environmentId;
        Name = name;
        Key = key;
    }

    public static Workflow Create(Environment environment, string? name, string? key)
    {
        var objectName = ObjectName.Create(name).Assert();
        var objectKey = ObjectKey.Create(key).Assert();
        
        var id = IdentityProvider.New<WorkflowId>();

        var workflow = new Workflow(
            id,
            environment.Id,
            objectName,
            objectKey);

        workflow.CreateDraftVersion();
        workflow.SetCreated();

        return workflow;
    }

    public WorkflowVersion CreateDraftVersion()
    {
        DomainErrors.Workflow.DraftAlreadyExists.Assert(() => DraftVersionId is null);

        var versionNumber = _versions.NextVersionNumber();

        var draft = PublishedVersion is null
            ? WorkflowVersion.CreateDraft(this, versionNumber)
            : WorkflowVersion.CreateDraftFrom(this, versionNumber, PublishedVersion);

        _versions.Add(draft);
        DraftVersionId = draft.Id;

        SetModified();

        return draft;
    }

    public WorkflowVersion PublishDraftVersion()
    {
        var draft = DraftVersion;

        DomainErrors.Workflow.NoDraftVersion.Assert(() => draft != null);
        DomainErrors.Workflow.NoSteps.Assert(() => draft!.Steps.Any());

        PublishedVersion?.Archive();
        draft!.Publish();

        PublishedVersionId = draft.Id;
        DraftVersionId = null;

        SetModified();

        return draft;
    }

    public WorkflowInstance Trigger()
    {
        DomainErrors.Workflow.NoPublishedVersion.Assert(() => PublishedVersion != null);
        return WorkflowInstance.Create(this, PublishedVersion!);
    }

    public override string ToString()
    {
        return $"[{Id.Value}] {Key}";
    }
}