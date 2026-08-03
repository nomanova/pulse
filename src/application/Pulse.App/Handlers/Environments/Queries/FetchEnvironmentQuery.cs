using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using Pulse.App.Common.Authorization.Policies;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Dto.Environments;
using Pulse.App.Handlers.Environments.Common;
using Pulse.App.Handlers.Environments.Common.Specifications;
using Pulse.Domain.Aggregates.Environments;

namespace Pulse.App.Handlers.Environments.Queries;

public sealed record FetchEnvironmentQuery : IQuery<ErrorOr<EnvironmentDto>>
{
    public required EnvironmentId EnvironmentId { get; init; }
}

public sealed class FetchEnvironmentQueryAuthorizer : PermissionAuthorizer<FetchEnvironmentQuery>;

public sealed class FetchEnvironmentQueryHandler : IQueryHandler<FetchEnvironmentQuery, ErrorOr<EnvironmentDto>>
{
    private readonly IEnvironmentRepository _environmentRepository;

    public FetchEnvironmentQueryHandler(
        IEnvironmentRepository environmentRepository)
    {
        _environmentRepository = environmentRepository;
    }

    public async Task<ErrorOr<EnvironmentDto>> Handle(
        FetchEnvironmentQuery query, CancellationToken cancellationToken)
    {
        // Fetch environment
        var specification = new EnvironmentByIdSpecification(query.EnvironmentId);
        var environment = await _environmentRepository.SearchOne(specification, cancellationToken);

        if (environment == null)
        {
            return Error.NotFound();
        }

        return environment.ToDto();
    }
}