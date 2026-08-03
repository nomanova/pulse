using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using Pulse.App.Common.Authorization.Policies;
using Pulse.App.Common.Database;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Handlers.Organizations.Common;
using Pulse.App.Handlers.Organizations.Common.Specifications;
using Pulse.Domain.Aggregates.Organizations;

namespace Pulse.App.Handlers.Organizations.Commands;

public sealed class RemoveOrganizationCommand : ICommand<ErrorOr<Success>>
{
    public required OrganizationId OrganizationId { get; init; }
}

public sealed class RemoveOrganizationCommandAuthorizer : PermissionAuthorizer<RemoveOrganizationCommand>;

public class RemoveOrganizationCommandHandler : ICommandHandler<RemoveOrganizationCommand, ErrorOr<Success>>
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveOrganizationCommandHandler(
        IOrganizationRepository organizationRepository,
        IUnitOfWork unitOfWork)
    {
        _organizationRepository = organizationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Success>> Handle(RemoveOrganizationCommand command, CancellationToken cancellationToken)
    {
        // Fetch organization
        var specification = new OrganizationByIdSpecification(command.OrganizationId);
        var organization = await _organizationRepository.SearchOne(specification, cancellationToken);

        if (organization == null)
        {
            return Error.NotFound();
        }

        // Remove
        _organizationRepository.Remove(organization);
        await _unitOfWork.Commit(cancellationToken);

        return Result.Success;
    }
}