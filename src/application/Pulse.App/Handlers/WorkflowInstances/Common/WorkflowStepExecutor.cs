using System.Threading;
using System.Threading.Tasks;
using Pulse.App.Common.Services.Interfaces;
using Pulse.Domain.Aggregates.WorkflowInstances;
using Pulse.Domain.Aggregates.WorkflowInstances.Entities;

namespace Pulse.App.Handlers.WorkflowInstances.Common;

public sealed class WorkflowStepExecutor : IWorkflowStepExecutor
{
    private readonly IPluginManager _pluginManager;

    public WorkflowStepExecutor(IPluginManager pluginManager)
    {
        _pluginManager = pluginManager;
    }

    public Task Execute(
        WorkflowInstanceId workflowInstanceId, 
        WorkflowInstanceStepId workflowInstanceStepId,
        CancellationToken cancellationToken = default)
    {
        // resolve workflow version step
        // get plugin id from workflow version step
        // get plugin parameters from workflow version step
        // invoke plugin
        
        // TODO - Implement
        
        return Task.CompletedTask;
    }
}