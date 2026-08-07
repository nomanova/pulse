using Pulse.App.Common.Database;
using Pulse.Domain.Aggregates.Workflows.Entities;

namespace Pulse.App.Handlers.Workflows.Common;

public interface IWorkflowVersionRepository : IReadOnlyRepository<WorkflowVersion>;

internal sealed class WorkflowVersionRepository : ReadRepository<WorkflowVersion>, IWorkflowVersionRepository
{
    public WorkflowVersionRepository(IDatabaseContext context) : base(context.WorkflowVersions)
    {
    }
}