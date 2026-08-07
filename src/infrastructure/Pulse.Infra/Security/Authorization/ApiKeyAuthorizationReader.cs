using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pulse.App.Common.Database;
using Pulse.App.Common.Security.Interfaces;
using Pulse.Domain.Aggregates.Environments;
using Pulse.Domain.Aggregates.WorkflowInstances;
using Pulse.Domain.Aggregates.Workflows;

namespace Pulse.Infra.Security.Authorization;

internal sealed class ApiKeyAuthorizationReader : IApiKeyAuthorizationReader
{
    private readonly IDatabaseContext _context;

    public ApiKeyAuthorizationReader(IDatabaseContext context)
    {
        _context = context;
    }

    public Task<bool> HasValidApiKeyForEnvironment(
        EnvironmentId environmentId,
        string apiKey,
        CancellationToken cancellationToken)
    {
        return (
            from environment in _context.Environments.IgnoreAutoIncludes()
            where environment.Id == environmentId
                  && !environment.IsDeleted
                  && (environment.ApiKey.Primary == apiKey || environment.ApiKey.Secondary == apiKey)
            select environment.Id
        ).AnyAsync(cancellationToken);
    }

    public Task<bool> HasValidApiKeyForWorkflow(
        WorkflowId workflowId,
        string apiKey,
        CancellationToken cancellationToken)
    {
        return (
            from workflow in _context.Workflows.IgnoreAutoIncludes()
            join environment in _context.Environments.IgnoreAutoIncludes()
                on workflow.EnvironmentId equals environment.Id
            where workflow.Id == workflowId
                  && !workflow.IsDeleted
                  && !environment.IsDeleted
                  && (environment.ApiKey.Primary == apiKey || environment.ApiKey.Secondary == apiKey)
            select workflow.Id
        ).AnyAsync(cancellationToken);
    }

    public Task<bool> HasValidApiKeyForWorkflowInstance(
        WorkflowInstanceId workflowInstanceId,
        string apiKey,
        CancellationToken cancellationToken)
    {
        return (
            from workflowInstance in _context.WorkflowInstances.IgnoreAutoIncludes()
            join workflowVersion in _context.WorkflowVersions.IgnoreAutoIncludes()
                on workflowInstance.WorkflowVersionId equals workflowVersion.Id
            join workflow in _context.Workflows.IgnoreAutoIncludes()
                on workflowVersion.WorkflowId equals workflow.Id
            join environment in _context.Environments.IgnoreAutoIncludes()
                on workflow.EnvironmentId equals environment.Id
            where workflowInstance.Id == workflowInstanceId
                  && !workflowInstance.IsDeleted
                  && !workflow.IsDeleted
                  && !environment.IsDeleted
                  && (environment.ApiKey.Primary == apiKey || environment.ApiKey.Secondary == apiKey)
            select workflowInstance.Id
        ).AnyAsync(cancellationToken);
    }
}