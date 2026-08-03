using System.Threading;
using System.Threading.Tasks;
using Pulse.Domain.Aggregates.WorkflowInstances;
using Pulse.Domain.Aggregates.WorkflowInstances.Entities;

namespace Pulse.App.Handlers.WorkflowInstances.Common;

public sealed class WorkflowStepExecutor : IWorkflowStepExecutor
{
    public Task Execute(
        WorkflowInstanceId workflowInstanceId, 
        WorkflowInstanceStepId workflowInstanceStepId,
        CancellationToken cancellationToken = default)
    {
        // TODO - Implement
        
        return Task.CompletedTask;
    }
}