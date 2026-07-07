using System.Collections.Generic;
using System.Linq;
using Pulse.Domain.Aggregates.Environments;
using Pulse.Domain.Aggregates.Organizations;
using Pulse.Domain.Aggregates.WorkflowInstances;
using Pulse.Domain.Aggregates.Workflows.Entities;
using Pulse.Domain.Common.Errors;
using Pulse.Domain.Common.Extensions;
using Pulse.Domain.Common.Models.Entities;
using Pulse.Domain.Common.Models.Text;
using Pulse.Domain.Common.Services;
using ApplicationId = Pulse.Domain.Aggregates.Applications.ApplicationId;
using Environment = Pulse.Domain.Aggregates.Environments.Environment;

namespace Pulse.Domain.Aggregates.Workflows;

public sealed record WorkflowId : EntityId<WorkflowId, Workflow>;

public sealed class Workflow : DomainEntity<WorkflowId>, IEnvironmentScoped, INamed
{
    public OrganizationId OrganizationId { get; private set; } = null!;

    public ApplicationId ApplicationId { get; private set; } = null!;

    public EnvironmentId EnvironmentId { get; private set; } = null!;

    public string Name { get; private set; } = null!;

    public string NormalizedName { get; private set; } = null!;

    public WorkflowVersionId? PublishedVersionId { get; private set; }
    
    private readonly List<WorkflowVersion> _versions = [];

    public IReadOnlyCollection<WorkflowVersion> Versions => _versions
        .OrderBy(version => version.Version)
        .ToList()
        .AsReadOnly();

    public WorkflowVersion? PublishedVersion => PublishedVersionId is null
        ? null
        : _versions.Single(version => version.Id == PublishedVersionId);
    
    private Workflow()
    {
    }

    private Workflow(
        WorkflowId id,
        OrganizationId organizationId,
        ApplicationId applicationId,
        EnvironmentId environmentId,
        string name,
        string normalizedName) : base(id)
    {
        OrganizationId = organizationId;
        ApplicationId = applicationId;
        EnvironmentId = environmentId;
        Name = name;
        NormalizedName = normalizedName;
    }

    public static Workflow Create(Environment environment, string? name)
    {
        var nameValue = name.AsName().Assert();
        var id = IdentityProvider.New<WorkflowId>();

        var workflow = new Workflow(
            id,
            environment.OrganizationId,
            environment.ApplicationId,
            environment.Id,
            nameValue,
            nameValue.AsNormalizedQueryable());

        workflow.CreateDraftVersion();

        workflow.SetCreated();

        return workflow;
    }

    public void Rename(string? name)
    {
        var nameValue = name.AsName().Assert();

        if (Name == nameValue)
        {
            return;
        }

        Name = nameValue;
        NormalizedName = nameValue.AsNormalizedQueryable();

        SetModified();
    }

    public WorkflowVersion CreateDraftVersion()
    {
        DomainErrors.Workflow.DraftAlreadyExists.Assert(() =>
            !_versions.Any(version => version.IsDraft));

        var versionNumber = _versions.Count == 0
            ? 1
            : _versions.Max(version => version.Version) + 1;

        var draft = PublishedVersion is null
            ? WorkflowVersion.CreateDraft(this, versionNumber)
            : WorkflowVersion.CreateDraftFrom(this, versionNumber, PublishedVersion);

        _versions.Add(draft);

        SetModified();

        return draft;
    }

    public WorkflowVersion GetDraftVersion()
    {
        var draft = _versions.SingleOrDefault(version => version.IsDraft);

        DomainErrors.Workflow.NoDraftVersion.Assert(() => draft != null);

        return draft!;
    }

    public WorkflowVersion PublishDraftVersion()
    {
        var draft = GetDraftVersion();

        DomainErrors.Workflow.NoSteps.Assert(() => draft.Steps.Any());

        PublishedVersion?.Archive();
        draft.Publish();

        PublishedVersionId = draft.Id;

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
        return $"[{Id.Value}] {Name}";
    }
}