using System.Threading;
using System.Threading.Tasks;
using Pulse.App.Common.Services.Interfaces;
using Pulse.App.Handlers.Workflows.Common;
using Pulse.App.Handlers.Workflows.Common.Specifications;
using Pulse.Domain.Aggregates.WorkflowInstances;
using Pulse.Domain.Aggregates.WorkflowInstances.Entities;

namespace Pulse.App.Handlers.WorkflowInstances.Common;

public sealed class WorkflowStepExecutor : IWorkflowStepExecutor
{
    private readonly IPluginManager _pluginManager;
    private readonly IWorkflowVersionRepository _workflowVersionRepository;

    public WorkflowStepExecutor(
        IPluginManager pluginManager,
        IWorkflowVersionRepository workflowVersionRepository)
    {
        _pluginManager = pluginManager;
        _workflowVersionRepository = workflowVersionRepository;
    }

    public async Task Execute(
        WorkflowInstance workflowInstance,
        WorkflowInstanceStep workflowInstanceStep,
        CancellationToken cancellationToken = default)
    {
        var workflowVersionId = workflowInstance.WorkflowVersionId;

        var specification = new WorkflowVersionByIdSpecification(workflowVersionId);
        var workflowVersion = await _workflowVersionRepository.SearchOne(specification, cancellationToken);

        if (workflowVersion == null)
        {
            return;
        }

        var workflowVersionStep = workflowVersion.GetStep(workflowInstanceStep.WorkflowVersionStepId);

        if (workflowVersionStep == null)
        {
            return;
        }

        //var result = await _pluginManager.InvokeAsync()

        // get plugin id from workflow version step
        // get plugin parameters from workflow version step
        // invoke plugin
    }
}