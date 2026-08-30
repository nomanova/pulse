using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using Pulse.App.Common.Authorization.Policies;
using Pulse.App.Common.Database;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Handlers.Connections.Common;
using Pulse.App.Handlers.Connections.Common.Specifications;
using Pulse.Domain.Aggregates.Connections;

namespace Pulse.App.Handlers.Connections.Commands;

public sealed record RemoveConnectionCommand : ICommand<ErrorOr<Success>>
{
    public required ConnectionId ConnectionId { get; init; }
}

public sealed class RemoveConnectionCommandAuthorizer : PermissionAuthorizer<RemoveConnectionCommand>;

public sealed class RemoveConnectionCommandHandler : ICommandHandler<RemoveConnectionCommand, ErrorOr<Success>>
{
    private readonly IConnectionRepository _connectionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveConnectionCommandHandler(
        IConnectionRepository connectionRepository,
        IUnitOfWork unitOfWork)
    {
        _connectionRepository = connectionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Success>> Handle(RemoveConnectionCommand command, CancellationToken cancellationToken)
    {
        // Fetch connection
        var specification = new ConnectionByIdSpecification(command.ConnectionId);
        var connection = await _connectionRepository.SearchOne(specification, cancellationToken);

        if (connection is null)
        {
            return Error.NotFound();
        }

        // Remove
        _connectionRepository.Remove(connection);
        await _unitOfWork.Commit(cancellationToken);

        return Result.Success;
    }
}