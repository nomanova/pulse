using Pulse.App.Common.Database.Specifications;
using Pulse.Domain.Aggregates.WorkflowInstances;

namespace Pulse.App.Handlers.WorkflowInstances.Common.Specifications;

public sealed class WorkflowInstanceByIdSpecification(
    WorkflowInstanceId id,
    bool includeDeleted = false) : ByIdSpecification<WorkflowInstance, WorkflowInstanceId>(id, includeDeleted);