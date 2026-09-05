using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using Pulse.App.Common.Authorization.Policies;
using Pulse.App.Common.Database.Specifications;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Common.Requests;
using Pulse.App.Dto.Common;
using Pulse.App.Dto.Connections;
using Pulse.App.Handlers.Connections.Common;
using Pulse.App.Handlers.Connections.Common.Specifications;
using Pulse.Domain.Aggregates.Connections;
using Pulse.Domain.Aggregates.Environments;
using Pulse.Domain.Common.Models.Entities;

namespace Pulse.App.Handlers.Connections.Queries;

public sealed record SearchConnectionsQuery : SearchQuery<ConnectionDto>
{
    public required EnvironmentId EnvironmentId { get; init; }
}

public sealed class SearchConnectionsQueryValidator : SearchQueryValidator<SearchConnectionsQuery, ConnectionDto>;

public sealed class SearchConnectionsQueryAuthorizer : PermissionAuthorizer<SearchConnectionsQuery>;

public class SearchConnectionsQueryHandler :
    IQueryHandler<SearchConnectionsQuery, ErrorOr<PagedSearchResultDto<ConnectionDto>>>
{
    private readonly IConnectionRepository _connectionRepository;

    public SearchConnectionsQueryHandler(IConnectionRepository connectionRepository)
    {
        _connectionRepository = connectionRepository;
    }

    public async Task<ErrorOr<PagedSearchResultDto<ConnectionDto>>> Handle(
        SearchConnectionsQuery query, CancellationToken cancellationToken)
    {
        var lastId = query.LastId?.AsIdentity<ConnectionId>();

        var orderBySpecification =
            new OrderByIdSpecification<Connection, ConnectionId>(query.Ascending);

        var searchBySpecification = new SearchConnectionsSpecification(query.EnvironmentId);

        var searchLastSpecification = lastId == null
            ? null
            : new ConnectionByIdSpecification(lastId);

        var searchResult = await _connectionRepository.SearchCursor(
            searchBySpecification,
            orderBySpecification,
            query.PageSize,
            searchLastSpecification,
            cancellationToken);

        return new PagedSearchResultDto<ConnectionDto>
        {
            HasNext = searchResult.HasNext,
            Entities = searchResult.Entities.Select(connection => connection.ToDto()).ToList()
        };
    }
}