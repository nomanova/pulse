using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using Pulse.App.Common.Authorization.Policies;
using Pulse.App.Common.Database;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Common.Mappers;
using Pulse.App.Common.Security.Interfaces;
using Pulse.App.Dto.Common;
using Pulse.App.Handlers.Applications.Common;
using Pulse.App.Handlers.Applications.Common.Specifications;
using Pulse.App.Handlers.Environments.Common;
using Pulse.App.Handlers.Memberships.Common;
using Pulse.Domain.Aggregates.Applications;
using Pulse.Domain.Aggregates.Environments;
using Pulse.Domain.Aggregates.Memberships;
using Pulse.Domain.Aggregates.Organizations;
using Pulse.Domain.Aggregates.Roles;

namespace Pulse.App.Handlers.Environments.Commands;

public sealed record CreateEnvironmentCommand :
    IApplicationScoped, ICommand<ErrorOr<IdentityDto>>
{
    public required OrganizationId OrganizationId { get; init; }

    public required ApplicationId ApplicationId { get; init; }

    public string? Name { get; init; }
}

public sealed class CreateEnvironmentCommandAuthorizer : PermissionAuthorizer<CreateEnvironmentCommand>;

public class CreateApplicationCommandHandler : ICommandHandler<CreateEnvironmentCommand, ErrorOr<IdentityDto>>
{
    private readonly ICachedUserProvider _cachedUserProvider;
    private readonly IApplicationRepository _applicationRepository;
    private readonly IEnvironmentRepository _environmentRepository;
    private readonly IMembershipRepository _membershipRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateApplicationCommandHandler(
        ICachedUserProvider cachedUserProvider,
        IApplicationRepository applicationRepository,
        IEnvironmentRepository environmentRepository,
        IMembershipRepository membershipRepository,
        IUnitOfWork unitOfWork)
    {
        _cachedUserProvider = cachedUserProvider;
        _applicationRepository = applicationRepository;
        _environmentRepository = environmentRepository;
        _membershipRepository = membershipRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<IdentityDto>> Handle(CreateEnvironmentCommand command,
        CancellationToken cancellationToken)
    {
        var user = await _cachedUserProvider.Get(cancellationToken);

        // Find application
        var application = await _applicationRepository.SearchOne(
            new ApplicationByIdSpecification(command.OrganizationId, command.ApplicationId),
            cancellationToken);

        if (application is null)
        {
            return Error.NotFound();
        }

        // Create environment
        var environment = Environment.Create(command.Name, application);
        _environmentRepository.Add(environment);

        // Set the creating user as the initial owner of the environment
        var membership = Membership.Create(user, Role.BuiltIn.EnvOwner, environment);
        _membershipRepository.Add(membership);

        await _unitOfWork.Commit(cancellationToken);

        return environment.ToIdentityDto();
    }
}