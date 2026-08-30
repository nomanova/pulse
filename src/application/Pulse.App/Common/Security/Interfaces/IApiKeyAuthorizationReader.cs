using System.Threading;
using System.Threading.Tasks;
using Pulse.Domain.Aggregates.Environments;
using Pulse.Domain.Aggregates.WorkflowInstances;
using Pulse.Domain.Aggregates.Workflows;
using Pulse.Domain.Aggregates.Workflows.Entities;

namespace Pulse.App.Common.Security.Interfaces;

public interface IApiKeyAuthorizationReader
{
    Task<bool> BelongsToEnvironment(
        WorkflowId workflowId, EnvironmentId environmentId, CancellationToken cancellationToken);

    Task<bool> BelongsToEnvironment(
        WorkflowVersionId workflowVersionId, EnvironmentId environmentId, CancellationToken cancellationToken);

    Task<bool> BelongsToEnvironment(
        WorkflowInstanceId workflowInstanceId, EnvironmentId environmentId, CancellationToken cancellationToken);
}