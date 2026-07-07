using System.Threading;
using System.Threading.Tasks;
using Pulse.Domain.Aggregates.WorkflowInstances;
using Pulse.Domain.Aggregates.WorkflowInstances.Entities;
using Pulse.Domain.Aggregates.Workflows.Entities;

namespace Pulse.App.Handlers.WorkflowInstances.Common;

public sealed class WorkflowStepExecutor : IWorkflowStepExecutor
{
    public Task Execute(
        WorkflowInstanceId workflowInstanceId, 
        WorkflowInstanceStepId workflowInstanceStepId,
        WorkflowVersionStepId workflowVersionStepId, 
        CancellationToken cancellationToken = default)
    {
        // TODO - Implement
        
        return Task.CompletedTask;
    }
}