using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using Pulse.App.Common.Authorization.Policies;
using Pulse.App.Common.Database.Specifications.Base;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Dto.Workflows;
using Pulse.App.Handlers.Workflows.Common;
using Pulse.Domain.Aggregates.Workflows.Entities;

namespace Pulse.App.Handlers.Workflows.Queries.Versions;

public sealed record FetchWorkflowVersionQuery : IQuery<ErrorOr<WorkflowVersionDto>>
{
    public required WorkflowVersionId WorkflowVersionId { get; init; }
}

public sealed class FetchWorkflowVersionQueryAuthorizer : PermissionAuthorizer<FetchWorkflowVersionQuery>;

public sealed class
    FetchWorkflowVersionQueryHandler : IQueryHandler<FetchWorkflowVersionQuery, ErrorOr<WorkflowVersionDto>>
{
    private readonly IWorkflowVersionRepository _workflowVersionRepository;

    private sealed class WorkflowVersionByIdSpecification(WorkflowVersionId id) : Specification<WorkflowVersion>
    {
        public override Expression<Func<WorkflowVersion, bool>> ToExpression()
        {
            return workflowVersion => workflowVersion.Id == id;
        }
    }

    public FetchWorkflowVersionQueryHandler(IWorkflowVersionRepository workflowVersionRepository)
    {
        _workflowVersionRepository = workflowVersionRepository;
    }

    public async Task<ErrorOr<WorkflowVersionDto>> Handle(FetchWorkflowVersionQuery query,
        CancellationToken cancellationToken)
    {
        var specification = new WorkflowVersionByIdSpecification(query.WorkflowVersionId);
        var workflowVersion = await _workflowVersionRepository.SearchOne(specification, cancellationToken);

        if (workflowVersion == null)
        {
            return Error.NotFound();
        }

        return workflowVersion.ToDto();
    }
}