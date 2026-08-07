using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using Pulse.App.Common.Authorization.Policies;
using Pulse.App.Common.Database.Specifications;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Common.Requests;
using Pulse.App.Dto.Common;
using Pulse.App.Dto.Workflows;
using Pulse.App.Handlers.Workflows.Common;
using Pulse.App.Handlers.Workflows.Common.Specifications;
using Pulse.Domain.Aggregates.Workflows;
using Pulse.Domain.Aggregates.Workflows.Entities;
using Pulse.Domain.Aggregates.Workflows.Enums;
using Pulse.Domain.Common.Models.Entities;

namespace Pulse.App.Handlers.Workflows.Queries.Versions;

public sealed record SearchWorkflowVersionsQuery : SearchQuery<WorkflowVersionDto>
{
    public required WorkflowId WorkflowId { get; init; }

    public WorkflowVersionStatus? Status { get; init; }
}

public sealed class
    SearchWorkflowVersionsQueryValidator : SearchQueryValidator<SearchWorkflowVersionsQuery, WorkflowVersionDto>;

public sealed class SearchWorkflowVersionsQueryAuthorizer : PermissionAuthorizer<SearchWorkflowVersionsQuery>;

public class SearchWorkflowVersionsQueryHandler :
    IQueryHandler<SearchWorkflowVersionsQuery, ErrorOr<PagedSearchResultDto<WorkflowVersionDto>>>
{
    private readonly IWorkflowVersionRepository _workflowVersionRepository;

    public SearchWorkflowVersionsQueryHandler(IWorkflowVersionRepository workflowVersionRepository)
    {
        _workflowVersionRepository = workflowVersionRepository;
    }

    public async Task<ErrorOr<PagedSearchResultDto<WorkflowVersionDto>>> Handle(
        SearchWorkflowVersionsQuery query, CancellationToken cancellationToken)
    {
        var lastId = query.LastId?.AsIdentity<WorkflowVersionId>();

        var orderBySpecification =
            new OrderByIdSpecification<WorkflowVersion, WorkflowVersionId>(query.Ascending);

        var searchBySpecification =
            new SearchWorkflowVersionsSpecification(query.WorkflowId, query.Status);

        var searchLastSpecification = lastId == null
            ? null
            : new WorkflowVersionByIdSpecification(lastId);

        var searchResult = await _workflowVersionRepository.SearchCursor(
            searchBySpecification,
            orderBySpecification,
            query.PageSize,
            searchLastSpecification,
            cancellationToken);

        return new PagedSearchResultDto<WorkflowVersionDto>
        {
            HasNext = searchResult.HasNext,
            Entities = searchResult.Entities.Select(workflow => workflow.ToDto()).ToList()
        };
    }
}