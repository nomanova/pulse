using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using Pulse.App.Common.Authorization.Policies;
using Pulse.App.Common.Context;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Dto.Environments;
using Pulse.App.Handlers.Environments.Common;
using Pulse.App.Handlers.Environments.Common.Specifications;

namespace Pulse.App.Handlers.Environments.Queries;

public sealed record FetchEnvironmentQuery : IApplicationRequest, IQuery<ErrorOr<EnvironmentDto>>
{
    public string? OrganizationName { get; init; }

    public string? ApplicationName { get; init; }

    public string? EnvironmentName { get; init; }
}

public sealed class FetchEnvironmentQueryAuthorizer : PermissionAuthorizer<FetchEnvironmentQuery>;

public sealed class FetchEnvironmentQueryHandler : IQueryHandler<FetchEnvironmentQuery, ErrorOr<EnvironmentDto>>
{
    private readonly IContextProvider _contextProvider;
    private readonly IEnvironmentRepository _environmentRepository;

    public FetchEnvironmentQueryHandler(
        IContextProvider contextProvider,
        IEnvironmentRepository environmentRepository)
    {
        _contextProvider = contextProvider;
        _environmentRepository = environmentRepository;
    }

    public async Task<ErrorOr<EnvironmentDto>> Handle(
        FetchEnvironmentQuery query, CancellationToken cancellationToken)
    {
        var organization = _contextProvider.Organization;
        var application = _contextProvider.Application;

        if (string.IsNullOrEmpty(query.ApplicationName))
        {
            return Error.NotFound();
        }

        // Fetch environment
        var specification = new EnvironmentByNameSpecification(organization.Id, application.Id, query.EnvironmentName);
        var environment = await _environmentRepository.SearchOne(specification, cancellationToken);

        if (environment == null)
        {
            return Error.NotFound();
        }

        return environment.ToDto();
    }
}