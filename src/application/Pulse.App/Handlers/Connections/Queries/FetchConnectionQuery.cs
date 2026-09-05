using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using Pulse.App.Common.Authorization.Policies;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Dto.Connections;
using Pulse.App.Handlers.Connections.Common;
using Pulse.App.Handlers.Connections.Common.Specifications;
using Pulse.Domain.Aggregates.Connections;

namespace Pulse.App.Handlers.Connections.Queries;

public sealed record FetchConnectionQuery : IQuery<ErrorOr<ConnectionDto>>
{
    public required ConnectionId ConnectionId { get; init; }
}

public sealed class FetchConnectionQueryAuthorizer : PermissionAuthorizer<FetchConnectionQuery>;

public sealed class FetchConnectionQueryHandler : IQueryHandler<FetchConnectionQuery, ErrorOr<ConnectionDto>>
{
    private readonly IConnectionRepository _connectionRepository;

    public FetchConnectionQueryHandler(IConnectionRepository connectionRepository)
    {
        _connectionRepository = connectionRepository;
    }

    public async Task<ErrorOr<ConnectionDto>> Handle(FetchConnectionQuery query, CancellationToken cancellationToken)
    {
        // Fetch connection
        var specification = new ConnectionByIdSpecification(query.ConnectionId);
        var connection = await _connectionRepository.SearchOne(specification, cancellationToken);

        if (connection is null)
        {
            return Error.NotFound();
        }

        return connection.ToDto();
    }
}