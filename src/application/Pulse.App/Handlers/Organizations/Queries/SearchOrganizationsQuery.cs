using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using Pulse.App.Common.Authorization.Policies;
using Pulse.App.Common.Database.Specifications;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Common.Errors;
using Pulse.App.Common.Requests;
using Pulse.App.Dto.Common;
using Pulse.App.Dto.Organizations;
using Pulse.App.Handlers.Organizations.Common;
using Pulse.App.Handlers.Organizations.Common.Specifications;
using Pulse.Domain.Aggregates.Organizations;
using Pulse.Domain.Common.Models.Entities;

namespace Pulse.App.Handlers.Organizations.Queries;

public sealed record SearchOrganizationsQuery : SearchQuery<OrganizationDto>;

public sealed class SearchOrganizationsQueryValidator : SearchQueryValidator<SearchOrganizationsQuery, OrganizationDto>;

public sealed class SearchOrganizationsQueryAuthorizer : PermissionAuthorizer<SearchOrganizationsQuery>;

public class SearchOrganizationsQueryHandler :
    IQueryHandler<SearchOrganizationsQuery, ErrorOr<PagedSearchResultDto<OrganizationDto>>>
{
    private static readonly List<string> OrderByProperties = [nameof(Organization.Name)];

    private readonly IOrganizationRepository _organizationRepository;

    public SearchOrganizationsQueryHandler(IOrganizationRepository organizationRepository)
    {
        _organizationRepository = organizationRepository;
    }

    public async Task<ErrorOr<PagedSearchResultDto<OrganizationDto>>> Handle(
        SearchOrganizationsQuery query, CancellationToken cancellationToken)
    {
        var lastId = query.LastId?.AsIdentity<OrganizationId>();

        if (query.OrderBy != null && !OrderByProperties.Contains(query.OrderBy))
        {
            return ApplicationErrors.OrderBy;
        }

        var orderBy = query.OrderBy ?? nameof(Organization.Name);

        var orderBySpecification = orderBy switch
        {
            nameof(Organization.Name) => new OrderBySpecification<Organization, OrganizationId, string>(orderBy,
                query.Ascending),
            _ => throw new NotImplementedException()
        };

        var searchBySpecification =
            new SearchOrganizationsSpecification(query.Query);

        var searchLastSpecification = lastId == null
            ? null
            : new OrganizationByIdSpecification(lastId);

        var searchResult = await _organizationRepository.SearchCursor(
            searchBySpecification,
            orderBySpecification,
            query.PageSize,
            searchLastSpecification,
            cancellationToken);

        return new PagedSearchResultDto<OrganizationDto>
        {
            HasNext = searchResult.HasNext,
            Entities = searchResult.Entities.Select(organization => organization.ToDto()).ToList()
        };
    }
}