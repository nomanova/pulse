using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using Pulse.App.Common.Authorization.Policies;
using Pulse.App.Common.Context;
using Pulse.App.Common.Database;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Common.Errors;
using Pulse.App.Common.Mappers;
using Pulse.App.Common.Security.Interfaces;
using Pulse.App.Dto.Common;
using Pulse.App.Handlers.Environments.Common;
using Pulse.App.Handlers.Environments.Common.Specifications;
using Pulse.App.Handlers.Memberships.Common;
using Pulse.Domain.Aggregates.Environments;
using Pulse.Domain.Aggregates.Memberships;
using Pulse.Domain.Aggregates.Roles;

namespace Pulse.App.Handlers.Environments.Commands;

public sealed record CreateEnvironmentCommand :
    IApplicationRequest, ICommand<ErrorOr<IdentityDto>>
{
    public required string? OrganizationName { get; init; }

    public required string? ApplicationName { get; init; }

    public required string? EnvironmentName { get; init; }
}

public sealed class CreateEnvironmentCommandAuthorizer : PermissionAuthorizer<CreateEnvironmentCommand>;

public class CreateApplicationCommandHandler : ICommandHandler<CreateEnvironmentCommand, ErrorOr<IdentityDto>>
{
    private readonly IUserProvider _userProvider;
    private readonly IContextProvider _contextProvider;
    private readonly IEnvironmentRepository _environmentRepository;
    private readonly IMembershipRepository _membershipRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateApplicationCommandHandler(
        IUserProvider userProvider,
        IContextProvider contextProvider,
        IEnvironmentRepository environmentRepository,
        IMembershipRepository membershipRepository,
        IUnitOfWork unitOfWork)
    {
        _userProvider = userProvider;
        _contextProvider = contextProvider;
        _environmentRepository = environmentRepository;
        _membershipRepository = membershipRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<IdentityDto>> Handle(CreateEnvironmentCommand command,
        CancellationToken cancellationToken)
    {
        var user = await _userProvider.Get(cancellationToken);
        var organization = _contextProvider.Organization;
        var application = _contextProvider.Application;

        // Duplicate name detection
        var existingEnvironment =
            await _environmentRepository.SearchOne(
                new EnvironmentByNameSpecification(organization.Id, application.Id, command.EnvironmentName),
                cancellationToken);

        if (existingEnvironment != null)
        {
            return ApplicationErrors.NameInUse;
        }

        // Create environment
        var environment = Environment.Create(command.EnvironmentName, application);
        _environmentRepository.Add(environment);

        // Set the creating user as the initial owner of the environment
        var membership = Membership.Create(user, Role.BuiltIn.EnvOwner, environment);
        _membershipRepository.Add(membership);

        await _unitOfWork.Commit(cancellationToken);

        return environment.ToIdentityDto();
    }
}