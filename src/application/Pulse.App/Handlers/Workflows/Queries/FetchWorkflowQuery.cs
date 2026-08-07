using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using Pulse.App.Common.Authorization.Policies;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Dto.Workflows;
using Pulse.App.Handlers.Workflows.Common;
using Pulse.App.Handlers.Workflows.Common.Specifications;
using Pulse.Domain.Aggregates.Workflows;

namespace Pulse.App.Handlers.Workflows.Queries;

public sealed record FetchWorkflowQuery : IQuery<ErrorOr<WorkflowDto>>
{
    public required WorkflowId WorkflowId { get; init; }
}

public sealed class FetchWorkflowQueryAuthorizer : ApiKeyAuthorizer<FetchWorkflowQuery>;

public sealed class FetchWorkflowQueryHandler : IQueryHandler<FetchWorkflowQuery, ErrorOr<WorkflowDto>>
{
    private readonly IWorkflowRepository _workflowRepository;

    public FetchWorkflowQueryHandler(IWorkflowRepository workflowRepository)
    {
        _workflowRepository = workflowRepository;
    }

    public async Task<ErrorOr<WorkflowDto>> Handle(FetchWorkflowQuery query, CancellationToken cancellationToken)
    {
        var specification = new WorkflowByIdSpecification(query.WorkflowId);
        var workflow = await _workflowRepository.SearchOne(specification, cancellationToken);

        if (workflow == null)
        {
            return Error.NotFound();
        }

        return workflow.ToDto();
    }
}