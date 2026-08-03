using Pulse.App.Common.Database.Specifications;
using Pulse.Domain.Aggregates.Workflows;

namespace Pulse.App.Handlers.Workflows.Common.Specifications;

public sealed class WorkflowByIdSpecification(WorkflowId id, bool includeDeleted = false) : 
    ByIdSpecification<Workflow, WorkflowId>(id, includeDeleted);