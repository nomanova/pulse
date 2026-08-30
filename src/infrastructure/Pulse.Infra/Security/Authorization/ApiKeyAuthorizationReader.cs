using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pulse.App.Common.Database;
using Pulse.App.Common.Security.Interfaces;
using Pulse.Domain.Aggregates.Environments;
using Pulse.Domain.Aggregates.WorkflowInstances;
using Pulse.Domain.Aggregates.Workflows;
using Pulse.Domain.Aggregates.Workflows.Entities;

namespace Pulse.Infra.Security.Authorization;

internal sealed class ApiKeyAuthorizationReader : IApiKeyAuthorizationReader
{
    private readonly IDatabaseContext _context;

    public ApiKeyAuthorizationReader(IDatabaseContext context)
    {
        _context = context;
    }

    public Task<bool> BelongsToEnvironment(
        WorkflowId workflowId, EnvironmentId environmentId, CancellationToken cancellationToken)
    {
        return (
            from workflow in _context.Workflows.IgnoreAutoIncludes()
            where workflow.Id == workflowId
                  && !workflow.IsDeleted
            select workflow.EnvironmentId
        ).AnyAsync(e => e == environmentId, cancellationToken);
    }
    
    public Task<bool> BelongsToEnvironment(
        WorkflowVersionId workflowVersionId, EnvironmentId environmentId, CancellationToken cancellationToken)
    {
        return (
            from workflowVersion in _context.WorkflowVersions.IgnoreAutoIncludes()
            join workflow in _context.Workflows.IgnoreAutoIncludes()
                on workflowVersion.WorkflowId equals workflow.Id
            where workflowVersion.Id == workflowVersionId
                  && !workflow.IsDeleted
            select workflow.EnvironmentId
        ).AnyAsync(e => e == environmentId, cancellationToken);
    }
    
    public Task<bool> BelongsToEnvironment(
        WorkflowInstanceId workflowInstanceId, EnvironmentId environmentId, CancellationToken cancellationToken)
    {
        return (
            from workflowInstance in _context.WorkflowInstances.IgnoreAutoIncludes()
            where workflowInstance.Id == workflowInstanceId
                  && !workflowInstance.IsDeleted
            select workflowInstance.EnvironmentId
        ).AnyAsync(e => e == environmentId, cancellationToken);
    }
}