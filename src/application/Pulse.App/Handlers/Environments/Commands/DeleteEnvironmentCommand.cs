using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using Pulse.App.Common.Authorization.Policies;
using Pulse.App.Common.Context;
using Pulse.App.Common.Database;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Handlers.Environments.Common;
using Pulse.App.Handlers.Environments.Common.Specifications;

namespace Pulse.App.Handlers.Environments.Commands;

public sealed class DeleteEnvironmentCommand : IApplicationRequest, ICommand<ErrorOr<Success>>
{
    public string? OrganizationName { get; init; }

    public string? ApplicationName { get; init; }

    public string? EnvironmentName { get; init; }
}

public sealed class DeleteEnvironmentCommandAuthorizer : PermissionAuthorizer<DeleteEnvironmentCommand>;

public sealed class DeleteEnvironmentCommandHandler : ICommandHandler<DeleteEnvironmentCommand, ErrorOr<Success>>
{
    private readonly IContextProvider _contextProvider;
    private readonly IEnvironmentRepository _environmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteEnvironmentCommandHandler(
        IContextProvider contextProvider,
        IEnvironmentRepository environmentRepository,
        IUnitOfWork unitOfWork)
    {
        _contextProvider = contextProvider;
        _environmentRepository = environmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Success>> Handle(DeleteEnvironmentCommand request, CancellationToken cancellationToken)
    {
        var organization = _contextProvider.Organization;
        var application = _contextProvider.Application;

        // Fetch environment
        var specification =
            new EnvironmentByNameSpecification(organization.Id, application.Id, request.EnvironmentName);
        var environment = await _environmentRepository.SearchOne(specification, cancellationToken);

        if (environment == null)
        {
            return Error.NotFound();
        }

        // Delete
        _environmentRepository.Remove(environment);
        await _unitOfWork.Commit(cancellationToken);

        return Result.Success;
    }
}