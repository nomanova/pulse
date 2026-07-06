using System;
using System.Collections.Generic;
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

    private readonly List<WorkflowStep> _steps = [];

    public IReadOnlyCollection<WorkflowStep> Steps => _steps.AsReadOnly();
    
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

        workflow.SetCreated();

        return workflow;
    }

    public WorkflowInstance Trigger()
    {
        throw new NotImplementedException();
    }

    public override string ToString()
    {
        return $"[{Id.Value}] {Name}";
    }
}