using System.Threading;
using System.Threading.Tasks;
using Pulse.Domain.Aggregates.Environments;
using Pulse.Domain.Aggregates.WorkflowInstances;
using Pulse.Domain.Aggregates.Workflows;
using Pulse.Domain.Aggregates.Workflows.Entities;

namespace Pulse.App.Common.Security.Interfaces;

public interface IApiKeyAuthorizationReader
{
    Task<bool> HasValidApiKeyForEnvironment(
        EnvironmentId environmentId,
        string apiKey,
        CancellationToken cancellationToken);
    
    Task<bool> HasValidApiKeyForWorkflow(
        WorkflowId workflowId,
        string apiKey,
        CancellationToken cancellationToken);

    Task<bool> HasValidApiKeyForWorkflowVersion(
        WorkflowVersionId workflowVersionId,
        string apiKey,
        CancellationToken cancellationToken);
    
    Task<bool> HasValidApiKeyForWorkflowInstance(
        WorkflowInstanceId workflowInstanceId,
        string apiKey,
        CancellationToken cancellationToken);
}