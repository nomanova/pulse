using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using Pulse.App.Common.Authorization.Policies;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Dto.Applications;
using Pulse.App.Handlers.Applications.Common;
using Pulse.App.Handlers.Applications.Common.Specifications;
using Pulse.Domain.Aggregates.Applications;

namespace Pulse.App.Handlers.Applications.Queries;

public sealed record FetchApplicationQuery : IQuery<ErrorOr<ApplicationDto>>
{
    public required ApplicationId ApplicationId { get; init; }
}

public sealed class FetchApplicationQueryAuthorizer : PermissionAuthorizer<FetchApplicationQuery>;

public sealed class FetchApplicationQueryHandler : IQueryHandler<FetchApplicationQuery, ErrorOr<ApplicationDto>>
{
    private readonly IApplicationRepository _applicationRepository;

    public FetchApplicationQueryHandler(IApplicationRepository applicationRepository)
    {
        _applicationRepository = applicationRepository;
    }

    public async Task<ErrorOr<ApplicationDto>> Handle(FetchApplicationQuery query, CancellationToken cancellationToken)
    {
        // Fetch application
        var specification = new ApplicationByIdSpecification(query.ApplicationId);
        var application = await _applicationRepository.SearchOne(specification, cancellationToken);

        if (application == null)
        {
            return Error.NotFound();
        }

        return application.ToDto();
    }
}