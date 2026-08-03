using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using Pulse.App.Common.Authorization.Policies;
using Pulse.App.Common.Database;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Handlers.Environments.Common;
using Pulse.App.Handlers.Environments.Common.Specifications;
using Pulse.Domain.Aggregates.Environments;

namespace Pulse.App.Handlers.Environments.Commands;

public sealed record RemoveEnvironmentCommand : ICommand<ErrorOr<Success>>
{
    public required EnvironmentId EnvironmentId { get; init; }
}

public sealed class RemoveEnvironmentCommandAuthorizer : PermissionAuthorizer<RemoveEnvironmentCommand>;

public sealed class RemoveEnvironmentCommandHandler : ICommandHandler<RemoveEnvironmentCommand, ErrorOr<Success>>
{
    private readonly IEnvironmentRepository _environmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveEnvironmentCommandHandler(
        IEnvironmentRepository environmentRepository,
        IUnitOfWork unitOfWork)
    {
        _environmentRepository = environmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Success>> Handle(RemoveEnvironmentCommand command, CancellationToken cancellationToken)
    {
        // Fetch environment
        var specification = new EnvironmentByIdSpecification(command.EnvironmentId);
        var environment = await _environmentRepository.SearchOne(specification, cancellationToken);

        if (environment == null)
        {
            return Error.NotFound();
        }

        // Remove
        _environmentRepository.Remove(environment);
        await _unitOfWork.Commit(cancellationToken);

        return Result.Success;
    }
}