using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using Pulse.App.Common.Authorization.Policies;
using Pulse.App.Common.Context;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Dto.Applications;
using Pulse.App.Handlers.Applications.Common;
using Pulse.App.Handlers.Applications.Common.Specifications;

namespace Pulse.App.Handlers.Applications.Queries;

public sealed record FetchApplicationQuery : IOrganizationRequest, IQuery<ErrorOr<ApplicationDto>>
{
    public required string? OrganizationName { get; init; }

    public required string? ApplicationName { get; init; }
}

public sealed class FetchApplicationQueryAuthorizer : PermissionAuthorizer<FetchApplicationQuery>;

public class FetchApplicationQueryHandler : IQueryHandler<FetchApplicationQuery, ErrorOr<ApplicationDto>>
{
    private readonly IContextProvider _contextProvider;
    private readonly IApplicationRepository _applicationRepository;

    public FetchApplicationQueryHandler(
        IContextProvider contextProvider,
        IApplicationRepository applicationRepository)
    {
        _contextProvider = contextProvider;
        _applicationRepository = applicationRepository;
    }

    public async Task<ErrorOr<ApplicationDto>> Handle(FetchApplicationQuery query, CancellationToken cancellationToken)
    {
        var organization = _contextProvider.Organization;

        if (string.IsNullOrEmpty(query.ApplicationName))
        {
            return Error.NotFound();
        }

        // Fetch application
        var specification = new ApplicationByNameSpecification(organization.Id, query.ApplicationName);
        var application = await _applicationRepository.SearchOne(specification, cancellationToken);

        if (application == null)
        {
            return Error.NotFound();
        }

        return application.ToDto();
    }
}