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
using Pulse.App.Dto.Applications;
using Pulse.App.Dto.Common;
using Pulse.App.Handlers.Applications.Common;
using Pulse.App.Handlers.Applications.Common.Specifications;
using Pulse.Domain.Aggregates.Applications;
using Pulse.Domain.Common.Models.Entities;
using ApplicationId = Pulse.Domain.Aggregates.Applications.ApplicationId;

namespace Pulse.App.Handlers.Applications.Queries;

public sealed record SearchApplicationsQuery : SearchQuery<ApplicationDto>, IOrganizationRequest
{
    public required string? OrganizationName { get; init; }
}

public sealed class SearchApplicationsQueryValidator : SearchQueryValidator<SearchApplicationsQuery, ApplicationDto>;

public sealed class SearchApplicationsQueryAuthorizer : PermissionAuthorizer<SearchApplicationsQuery>;

public class SearchApplicationsQueryHandler :
    IQueryHandler<SearchApplicationsQuery, ErrorOr<PagedSearchResultDto<ApplicationDto>>>
{
    private static readonly List<string> OrderByProperties = [nameof(Application.Name)];

    private readonly IContextProvider _contextProvider;
    private readonly IApplicationRepository _applicationRepository;

    public SearchApplicationsQueryHandler(
        IContextProvider contextProvider,
        IApplicationRepository applicationRepository)
    {
        _contextProvider = contextProvider;
        _applicationRepository = applicationRepository;
    }

    public async Task<ErrorOr<PagedSearchResultDto<ApplicationDto>>> Handle(
        SearchApplicationsQuery query, CancellationToken cancellationToken)
    {
        var organization = _contextProvider.Organization;

        var lastId = query.LastId?.AsIdentity<ApplicationId>();

        if (query.OrderBy != null && !OrderByProperties.Contains(query.OrderBy))
        {
            return ApplicationErrors.OrderBy;
        }

        var orderBy = query.OrderBy ?? nameof(Application.Name);

        var orderBySpecification = orderBy switch
        {
            nameof(Application.Name) => new OrderBySpecification<Application, ApplicationId, string>(orderBy,
                query.Ascending),
            _ => throw new NotImplementedException()
        };

        var searchBySpecification =
            new SearchApplicationsSpecification(organization.Id, query.Query);

        var searchLastSpecification = lastId == null
            ? null
            : new ApplicationByIdSpecification(organization.Id, lastId);

        var searchResult = await _applicationRepository.SearchCursor(
            searchBySpecification,
            orderBySpecification,
            query.PageSize,
            searchLastSpecification,
            cancellationToken);

        return new PagedSearchResultDto<ApplicationDto>
        {
            HasNext = searchResult.HasNext,
            Entities = searchResult.Entities.Select(application => application.ToDto()).ToList()
        };
    }
}