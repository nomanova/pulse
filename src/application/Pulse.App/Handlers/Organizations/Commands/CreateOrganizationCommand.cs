using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using Pulse.App.Common.Authorization.Policies;
using Pulse.App.Common.Database;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Common.Errors;
using Pulse.App.Common.Mappers;
using Pulse.App.Common.Security.Interfaces;
using Pulse.App.Dto.Common;
using Pulse.App.Handlers.Memberships.Common;
using Pulse.App.Handlers.Organizations.Common;
using Pulse.App.Handlers.Organizations.Common.Specifications;
using Pulse.Domain.Aggregates.Memberships;
using Pulse.Domain.Aggregates.Organizations;
using Pulse.Domain.Aggregates.Roles;

namespace Pulse.App.Handlers.Organizations.Commands;

public sealed record CreateOrganizationCommand : ICommand<ErrorOr<IdentityDto>>
{
    public required string? OrganizationName { get; init; }
}

public sealed class CreateOrganizationCommandAuthorizer : PermissionAuthorizer<CreateOrganizationCommand>;

public class CreateOrganizationCommandHandler : ICommandHandler<CreateOrganizationCommand, ErrorOr<IdentityDto>>
{
    private readonly IUserProvider _userProvider;
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IMembershipRepository _membershipRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateOrganizationCommandHandler(
        IUserProvider userProvider,
        IOrganizationRepository organizationRepository,
        IMembershipRepository membershipRepository,
        IUnitOfWork unitOfWork)
    {
        _userProvider = userProvider;
        _organizationRepository = organizationRepository;
        _membershipRepository = membershipRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<IdentityDto>> Handle(CreateOrganizationCommand command,
        CancellationToken cancellationToken)
    {
        var user = await _userProvider.Get(cancellationToken);

        // Duplicate name detection
        var specification = new OrganizationByNameSpecification(command.OrganizationName);
        var existingOrganization = await _organizationRepository.SearchOne(specification, cancellationToken);

        if (existingOrganization != null)
        {
            return ApplicationErrors.NameInUse;
        }

        // Create organization
        var organization = Organization.Create(command.OrganizationName);
        _organizationRepository.Add(organization);

        // Set the creating user as the initial owner of the organization
        var membership = Membership.Create(user, Role.BuiltIn.OrgOwner, organization);
        _membershipRepository.Add(membership);

        await _unitOfWork.Commit(cancellationToken);

        return organization.ToIdentityDto();
    }
}