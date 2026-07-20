using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using Pulse.App.Common.Authorization.Policies;
using Pulse.App.Common.Context;
using Pulse.App.Common.Database;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Handlers.Applications.Common;
using Pulse.App.Handlers.Applications.Common.Specifications;

namespace Pulse.App.Handlers.Applications.Commands;

public sealed class DeleteApplicationCommand : IOrganizationRequest, ICommand<ErrorOr<Success>>
{
    public required string? OrganizationName { get; init; }

    public required string? ApplicationName { get; init; }
}

public sealed class DeleteApplicationCommandAuthorizer : PermissionAuthorizer<DeleteApplicationCommand>;

public class DeleteApplicationCommandHandler : ICommandHandler<DeleteApplicationCommand, ErrorOr<Success>>
{
    private readonly IContextProvider _contextProvider;
    private readonly IApplicationRepository _applicationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteApplicationCommandHandler(
        IContextProvider contextProvider,
        IApplicationRepository applicationRepository,
        IUnitOfWork unitOfWork)
    {
        _contextProvider = contextProvider;
        _applicationRepository = applicationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Success>> Handle(DeleteApplicationCommand command, CancellationToken cancellationToken)
    {
        var organization = _contextProvider.Organization;

        // Fetch application
        var specification = new ApplicationByNameSpecification(organization.Id, command.ApplicationName);
        var application = await _applicationRepository.SearchOne(specification, cancellationToken);

        if (application == null)
        {
            return Error.NotFound();
        }

        // Delete
        _applicationRepository.Remove(application);
        await _unitOfWork.Commit(cancellationToken);

        return Result.Success;
    }
}