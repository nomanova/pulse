using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using Pulse.App.Common.Authorization.Policies;
using Pulse.App.Common.Database;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Handlers.Applications.Common;
using Pulse.App.Handlers.Applications.Common.Specifications;
using Pulse.Domain.Aggregates.Applications;

namespace Pulse.App.Handlers.Applications.Commands;

public sealed record RemoveApplicationCommand : ICommand<ErrorOr<Success>>
{
    public required ApplicationId ApplicationId { get; init; }
}

public sealed class RemoveApplicationCommandAuthorizer : PermissionAuthorizer<RemoveApplicationCommand>;

public sealed class RemoveApplicationCommandHandler : ICommandHandler<RemoveApplicationCommand, ErrorOr<Success>>
{
    private readonly IApplicationRepository _applicationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveApplicationCommandHandler(
        IApplicationRepository applicationRepository,
        IUnitOfWork unitOfWork)
    {
        _applicationRepository = applicationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Success>> Handle(RemoveApplicationCommand command, CancellationToken cancellationToken)
    {
        // Fetch application
        var specification = new ApplicationByIdSpecification(command.ApplicationId);
        var application = await _applicationRepository.SearchOne(specification, cancellationToken);

        if (application == null)
        {
            return Error.NotFound();
        }

        // Remove
        _applicationRepository.Remove(application);
        await _unitOfWork.Commit(cancellationToken);

        return Result.Success;
    }
}