using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Pulse.Domain.Aggregates.Environments;
using Pulse.Domain.Aggregates.WorkflowInstances;
using Pulse.Domain.Aggregates.WorkflowInstances.Entities;
using Pulse.Domain.Aggregates.Workflows.Enums;
using Pulse.Domain.Aggregates.Workflows.ValueObjects;
using Pulse.Domain.Common.Exceptions;
using Pulse.Plugin.Providers;

namespace Pulse.App.Handlers.WorkflowInstances.Common;

public interface IWorkflowStepExecutor
{
    Task Execute(
        WorkflowInstance workflowInstance,
        WorkflowInstanceStep workflowInstanceStep,
        CancellationToken cancellationToken = default);
}

public sealed class WorkflowStepExecutor : IWorkflowStepExecutor
{
    private readonly IProviderResolver _providerResolver;

    public WorkflowStepExecutor(IProviderResolver providerResolver)
    {
        _providerResolver = providerResolver;
    }

    public async Task Execute(
        WorkflowInstance workflowInstance,
        WorkflowInstanceStep workflowInstanceStep,
        CancellationToken cancellationToken = default)
    {
        var environmentId = workflowInstance.EnvironmentId;
        var stepDefinition = workflowInstanceStep.Definition;

        switch (stepDefinition.Type)
        {
            case WorkflowStepDefinitionType.Provider:
                await ExecuteProvider(environmentId, (ProviderWorkflowStepDefinition)stepDefinition, cancellationToken);
                break;
            default:
                throw new NotImplementedException(stepDefinition.Type.ToString());
        }
    }

    private async Task ExecuteProvider(
        EnvironmentId environmentId,
        ProviderWorkflowStepDefinition stepDefinition,
        CancellationToken cancellationToken)
    {
        var context = await _providerResolver.TryResolveFor(
            environmentId, stepDefinition.Channel, cancellationToken);

        if (context == null)
        {
            throw new AppException($"Could not resolve provider for channel {stepDefinition.Channel}");
        }

        var request = new ProviderPluginInvocationRequest
        {
            ConnectionParameters = context.Connection.Parameters.ToList(),
            InvocationParameters = stepDefinition.Parameters.ToList()
        };

        await context.Plugin.Invoke(request, cancellationToken);
    }
}