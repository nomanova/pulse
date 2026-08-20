using System.Threading;
using System.Threading.Tasks;
using Pulse.Domain.Aggregates.WorkflowInstances;
using Pulse.Domain.Aggregates.WorkflowInstances.Entities;

namespace Pulse.App.Handlers.WorkflowInstances.Common;

public interface IWorkflowStepExecutor
{
    Task Execute(
        WorkflowInstance workflowInstance,
        WorkflowInstanceStep workflowInstanceStep,
        CancellationToken cancellationToken = default);
}