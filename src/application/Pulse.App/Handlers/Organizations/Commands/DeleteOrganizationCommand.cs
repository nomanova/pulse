using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using Pulse.App.Common.Authorization.Policies;
using Pulse.App.Common.Database;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Handlers.Organizations.Common;
using Pulse.App.Handlers.Organizations.Common.Specifications;

namespace Pulse.App.Handlers.Organizations.Commands;

public sealed class DeleteOrganizationCommand : ICommand<ErrorOr<Success>>
{
    public required string? OrganizationName { get; init; }
}

public sealed class DeleteOrganizationCommandAuthorizer : PermissionAuthorizer<DeleteOrganizationCommand>;

public class DeleteOrganizationCommandHandler : ICommandHandler<DeleteOrganizationCommand, ErrorOr<Success>>
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteOrganizationCommandHandler(
        IOrganizationRepository organizationRepository,
        IUnitOfWork unitOfWork)
    {
        _organizationRepository = organizationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Success>> Handle(DeleteOrganizationCommand command, CancellationToken cancellationToken)
    {
        // Fetch organization
        var specification = new OrganizationByNameSpecification(command.OrganizationName);
        var organization = await _organizationRepository.SearchOne(specification, cancellationToken);

        if (organization == null)
        {
            return Error.NotFound();
        }

        // Delete
        _organizationRepository.Remove(organization);
        await _unitOfWork.Commit(cancellationToken);

        return Result.Success;
    }
}