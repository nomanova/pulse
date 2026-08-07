using Pulse.App.Common.Database.Specifications;
using Pulse.Domain.Aggregates.Workflows.Entities;

namespace Pulse.App.Handlers.Workflows.Common.Specifications;

public sealed class WorkflowVersionByIdSpecification(WorkflowVersionId id) : 
    ByEntityIdSpecification<WorkflowVersion, WorkflowVersionId>(id);