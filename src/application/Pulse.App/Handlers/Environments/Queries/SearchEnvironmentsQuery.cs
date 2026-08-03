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
using Pulse.App.Dto.Environments;
using Pulse.App.Handlers.Environments.Common;
using Pulse.App.Handlers.Environments.Common.Specifications;
using Pulse.Domain.Aggregates.Environments;
using Pulse.Domain.Common.Models.Entities;
using ApplicationId = Pulse.Domain.Aggregates.Applications.ApplicationId;
using Environment = Pulse.Domain.Aggregates.Environments.Environment;

namespace Pulse.App.Handlers.Environments.Queries;

public sealed record SearchEnvironmentsQuery : SearchQuery<EnvironmentDto>
{
    public required ApplicationId ApplicationId { get; init; }
}

public sealed class SearchEnvironmentsQueryValidator : SearchQueryValidator<SearchEnvironmentsQuery, EnvironmentDto>;

public sealed class SearchEnvironmentsQueryAuthorizer : PermissionAuthorizer<SearchEnvironmentsQuery>;

public class SearchEnvironmentsQueryHandler :
    IQueryHandler<SearchEnvironmentsQuery, ErrorOr<PagedSearchResultDto<EnvironmentDto>>>
{
    private static readonly List<string> OrderByProperties = [nameof(Environment.Name)];

    private readonly IEnvironmentRepository _environmentRepository;

    public SearchEnvironmentsQueryHandler(
        IEnvironmentRepository environmentRepository)
    {
        _environmentRepository = environmentRepository;
    }

    public async Task<ErrorOr<PagedSearchResultDto<EnvironmentDto>>> Handle(
        SearchEnvironmentsQuery query, CancellationToken cancellationToken)
    {
        var lastId = query.LastId?.AsIdentity<EnvironmentId>();

        if (query.OrderBy != null && !OrderByProperties.Contains(query.OrderBy))
        {
            return ApplicationErrors.OrderBy;
        }

        var orderBy = query.OrderBy ?? nameof(Environment.Name);

        var orderBySpecification = orderBy switch
        {
            nameof(Environment.Name) => new OrderBySpecification<Environment, EnvironmentId, string>(orderBy,
                query.Ascending),
            _ => throw new NotImplementedException()
        };

        var searchBySpecification =
            new SearchEnvironmentsSpecification(query.ApplicationId, query.Query);

        var searchLastSpecification = lastId == null
            ? null
            : new EnvironmentByIdSpecification(lastId);

        var searchResult = await _environmentRepository.SearchCursor(
            searchBySpecification,
            orderBySpecification,
            query.PageSize,
            searchLastSpecification,
            cancellationToken);

        return new PagedSearchResultDto<EnvironmentDto>
        {
            HasNext = searchResult.HasNext,
            Entities = searchResult.Entities.Select(environment => environment.ToDto()).ToList()
        };
    }
}