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
using Pulse.App.Handlers.Environments.Common;
using Pulse.App.Handlers.Environments.Common.Specifications;
using Pulse.App.Handlers.Memberships.Common;
using Pulse.Domain.Aggregates.Applications;
using Pulse.Domain.Aggregates.Environments;
using Pulse.Domain.Aggregates.Memberships;
using Pulse.Domain.Aggregates.Roles;

namespace Pulse.App.Handlers.Environments.Commands;

public sealed record AddEnvironmentCommand : ICommand<ErrorOr<IdentityDto>>
{
    public required ApplicationId ApplicationId { get; init; }
    
    public required string? EnvironmentName { get; init; }
}

public sealed class AddEnvironmentCommandAuthorizer : PermissionAuthorizer<AddEnvironmentCommand>;

public sealed class AddEnvironmentCommandHandler : ICommandHandler<AddEnvironmentCommand, ErrorOr<IdentityDto>>
{
    private readonly IUserProvider _userProvider;
    private readonly IApplicationRepository _applicationRepository;
    private readonly IEnvironmentRepository _environmentRepository;
    private readonly IMembershipRepository _membershipRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddEnvironmentCommandHandler(
        IUserProvider userProvider,
        IApplicationRepository applicationRepository,
        IEnvironmentRepository environmentRepository,
        IMembershipRepository membershipRepository,
        IUnitOfWork unitOfWork)
    {
        _userProvider = userProvider;
        _applicationRepository = applicationRepository;
        _environmentRepository = environmentRepository;
        _membershipRepository = membershipRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<IdentityDto>> Handle(AddEnvironmentCommand command,
        CancellationToken cancellationToken)
    {
        var user = await _userProvider.Get(cancellationToken);

        // Fetch application
        var applicationSpecification = new ApplicationByIdSpecification(command.ApplicationId);
        var application = await _applicationRepository.SearchOne(applicationSpecification, cancellationToken);

        if (application == null)
        {
            return Error.NotFound();
        }
        
        // Duplicate name detection
        var specification = new EnvironmentByNameSpecification(application.Id, command.EnvironmentName);
        var existingEnvironment = await _environmentRepository.SearchOne(specification, cancellationToken);

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