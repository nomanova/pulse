using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using Pulse.App.Common.Authorization.Policies;
using Pulse.App.Common.Context;
using Pulse.App.Common.Database.Specifications;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Common.Errors;
using Pulse.App.Common.Requests;
using Pulse.App.Dto.Common;
using Pulse.App.Dto.Workflows;
using Pulse.App.Handlers.Workflows.Common;
using Pulse.App.Handlers.Workflows.Common.Specifications;
using Pulse.Domain.Aggregates.Workflows;
using Pulse.Domain.Common.Models.Entities;

namespace Pulse.App.Handlers.Workflows.Queries;

public sealed record SearchWorkflowsQuery : SearchQuery<WorkflowDto>, IEnvironmentRequest
{
    public string? OrganizationName { get; init; }

    public string? ApplicationName { get; init; }

    public string? EnvironmentName { get; init; }
}

public sealed class SearchWorkflowsQueryValidator : SearchQueryValidator<SearchWorkflowsQuery, WorkflowDto>;

public sealed class SearchWorkflowsQueryAuthorizer : PermissionAuthorizer<SearchWorkflowsQuery>;

public class SearchWorkflowsQueryHandler :
    IQueryHandler<SearchWorkflowsQuery, ErrorOr<PagedSearchResultDto<WorkflowDto>>>
{
    private static readonly List<string> OrderByProperties = [nameof(Workflow.Name)];

    private readonly IContextProvider _contextProvider;
    private readonly IWorkflowRepository _workflowRepository;

    public SearchWorkflowsQueryHandler(
        IContextProvider contextProvider,
        IWorkflowRepository workflowRepository)
    {
        _contextProvider = contextProvider;
        _workflowRepository = workflowRepository;
    }

    public async Task<ErrorOr<PagedSearchResultDto<WorkflowDto>>> Handle(SearchWorkflowsQuery query,
        CancellationToken cancellationToken)
    {
        var organization = _contextProvider.Organization;
        var application = _contextProvider.Application;
        var environment = _contextProvider.Environment;

        var lastId = query.LastId?.AsIdentity<WorkflowId>();

        if (query.OrderBy != null && !OrderByProperties.Contains(query.OrderBy))
        {
            return ApplicationErrors.OrderBy;
        }

        var orderBy = query.OrderBy ?? nameof(Workflow.Name);

        var orderBySpecification = orderBy switch
        {
            nameof(Workflow.Name) => new OrderBySpecification<Workflow, WorkflowId, string>(orderBy,
                query.Ascending),
            _ => throw new NotImplementedException()
        };

        var searchBySpecification =
            new SearchWorkflowsSpecification(organization.Id, application.Id, environment.Id, query.Query);

        var searchLastSpecification = lastId == null
            ? null
            : new WorkflowByIdSpecification(organization.Id, application.Id, environment.Id, lastId);

        var searchResult = await _workflowRepository.SearchCursor(
            searchBySpecification,
            orderBySpecification,
            query.PageSize,
            searchLastSpecification,
            cancellationToken);

        return new PagedSearchResultDto<WorkflowDto>
        {
            HasNext = searchResult.HasNext,
            Entities = searchResult.Entities.Select(workflow => workflow.ToDto()).ToList()
        };
    }
}