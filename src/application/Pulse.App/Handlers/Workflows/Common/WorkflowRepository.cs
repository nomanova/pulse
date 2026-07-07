using Pulse.App.Common.Database;
using Pulse.Domain.Aggregates.Workflows;

namespace Pulse.App.Handlers.Workflows.Common;

public interface IWorkflowRepository : IRepository<Workflow>;

internal sealed class WorkflowRepository : Repository<Workflow>, IWorkflowRepository
{
    public WorkflowRepository(IDatabaseContext context) : base(context.Workflows)
    {
    }
}