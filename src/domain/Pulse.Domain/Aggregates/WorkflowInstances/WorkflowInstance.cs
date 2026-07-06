using Pulse.Domain.Aggregates.Applications;
using Pulse.Domain.Aggregates.Environments;
using Pulse.Domain.Aggregates.Organizations;
using Pulse.Domain.Aggregates.WorkflowInstances.Enums;
using Pulse.Domain.Aggregates.Workflows;
using Pulse.Domain.Common.Models.Entities;

namespace Pulse.Domain.Aggregates.WorkflowInstances;

public sealed record WorkflowInstanceId : EntityId<WorkflowInstanceId, WorkflowInstance>;

public sealed class WorkflowInstance : DomainEntity<WorkflowInstanceId>, IEnvironmentScoped
{
    public OrganizationId OrganizationId { get; private set; } = null!;

    public ApplicationId ApplicationId { get; private set; } = null!;

    public EnvironmentId EnvironmentId { get; private set; } = null!;

    public WorkflowId WorkflowId { get; private set; } = null!;
    
    public WorkflowInstanceStatus Status { get; private set; }
}