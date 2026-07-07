using Pulse.App.Common.Database;
using Pulse.Domain.Aggregates.WorkflowInstances;

namespace Pulse.App.Handlers.WorkflowInstances.Common;

public interface IWorkflowInstanceRepository : IRepository<WorkflowInstance>;

internal sealed class WorkflowInstanceRepository : Repository<WorkflowInstance>, IWorkflowInstanceRepository
{
    public WorkflowInstanceRepository(IDatabaseContext context) : base(context.WorkflowInstances)
    {
    }
}