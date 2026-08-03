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
using Pulse.App.Handlers.Applications.Common;
using Pulse.App.Handlers.Applications.Common.Specifications;
using Pulse.App.Handlers.Memberships.Common;
using Pulse.App.Handlers.Organizations.Common;
using Pulse.App.Handlers.Organizations.Common.Specifications;
using Pulse.Domain.Aggregates.Applications;
using Pulse.Domain.Aggregates.Memberships;
using Pulse.Domain.Aggregates.Organizations;
using Pulse.Domain.Aggregates.Roles;

namespace Pulse.App.Handlers.Applications.Commands;

public sealed record AddApplicationCommand : ICommand<ErrorOr<IdentityDto>>
{
    public required OrganizationId OrganizationId { get; init; }
    
    public required string? ApplicationName { get; init; }
}

public sealed class AddApplicationCommandAuthorizer : PermissionAuthorizer<AddApplicationCommand>;

public class AddApplicationCommandHandler : ICommandHandler<AddApplicationCommand, ErrorOr<IdentityDto>>
{
    private readonly IUserProvider _userProvider;
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IApplicationRepository _applicationRepository;
    private readonly IMembershipRepository _membershipRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddApplicationCommandHandler(
        IUserProvider userProvider,
        IOrganizationRepository organizationRepository,
        IApplicationRepository applicationRepository,
        IMembershipRepository membershipRepository,
        IUnitOfWork unitOfWork)
    {
        _userProvider = userProvider;
        _organizationRepository = organizationRepository;
        _applicationRepository = applicationRepository;
        _membershipRepository = membershipRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<IdentityDto>> Handle(AddApplicationCommand command,
        CancellationToken cancellationToken)
    {
        var user = await _userProvider.Get(cancellationToken);

        // Fetch organization
        var organizationSpecification = new OrganizationByIdSpecification(command.OrganizationId);
        var organization = await _organizationRepository.SearchOne(organizationSpecification, cancellationToken);

        if (organization == null)
        {
            return Error.NotFound();
        }

        // Duplicate name detection
        var applicationSpecification = new ApplicationByNameSpecification(organization.Id, command.ApplicationName); 
        var existingApplication = await _applicationRepository.SearchOne(applicationSpecification, cancellationToken);

        if (existingApplication != null)
        {
            return ApplicationErrors.NameInUse;
        }
       
        // Create application
        var application = Application.Create(command.ApplicationName, organization);
        _applicationRepository.Add(application);

        // Set the creating user as the initial owner of the application
        var membership = Membership.Create(user, Role.BuiltIn.AppOwner, application);
        _membershipRepository.Add(membership);

        await _unitOfWork.Commit(cancellationToken);

        return application.ToIdentityDto();
    }
}