using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using Pulse.App.Common.Database;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Common.Mappers;
using Pulse.App.Common.Security.Interfaces;
using Pulse.App.Dto.Common;
using Pulse.App.Handlers.Applications.Common;
using Pulse.App.Handlers.Memberships.Common;
using Pulse.App.Handlers.Organizations.Common;
using Pulse.App.Handlers.Organizations.Common.Specifications;
using Pulse.Domain.Aggregates.Applications;
using Pulse.Domain.Aggregates.Memberships;
using Pulse.Domain.Aggregates.Organizations;
using Pulse.Domain.Aggregates.Roles;

namespace Pulse.App.Handlers.Applications.Commands;

public class CreateApplicationCommandHandler : ICommandHandler<CreateApplicationCommand, ErrorOr<IdentityDto>>
{
    private readonly ICachedUserProvider _cachedUserProvider;
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IApplicationRepository _applicationRepository;
    private readonly IMembershipRepository _membershipRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateApplicationCommandHandler(
        ICachedUserProvider cachedUserProvider,
        IOrganizationRepository organizationRepository,
        IApplicationRepository applicationRepository,
        IMembershipRepository membershipRepository,
        IUnitOfWork unitOfWork)
    {
        _cachedUserProvider = cachedUserProvider;
        _organizationRepository = organizationRepository;
        _applicationRepository = applicationRepository;
        _membershipRepository = membershipRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<IdentityDto>> Handle(CreateApplicationCommand command,
        CancellationToken cancellationToken)
    {
        var user = await _cachedUserProvider.Get(cancellationToken);

        var organization = await _organizationRepository.SearchOne(
            new OrganizationByIdSpecification(OrganizationId.New(command.OrganizationId!)),
            cancellationToken);

        if (organization is null)
        {
            return Error.NotFound();
        }

        // Create application
        var application = Application.Create(command.Name, organization);
        _applicationRepository.Add(application);

        // Set the creating user as the initial owner of the application
        var membership = Membership.Create(user, Role.BuiltIn.AppOwner, application);
        _membershipRepository.Add(membership);

        await _unitOfWork.Commit(cancellationToken);

        return application.ToIdentityDto();
    }
}