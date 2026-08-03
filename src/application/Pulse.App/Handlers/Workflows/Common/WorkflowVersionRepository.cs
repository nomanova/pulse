using Pulse.App.Common.Database;
using Pulse.Domain.Aggregates.Workflows.Entities;

namespace Pulse.App.Handlers.Workflows.Common;

public interface IWorkflowVersionRepository : IReadOnlyRepository<WorkflowVersion>;

public class WorkflowVersionRepository : ReadRepository<WorkflowVersion>, IWorkflowVersionRepository
{
    protected WorkflowVersionRepository(IDatabaseContext context) : base(context.WorkflowVersions)
    {
    }
}