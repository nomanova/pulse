using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using Pulse.App.Common.Authorization.Policies;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Common.Errors;
using Pulse.App.Common.Security.Interfaces;
using Pulse.App.Dto.Users;
using Pulse.App.Handlers.Users.Common;
using Pulse.App.Handlers.Users.Common.Specifications;
using Pulse.Domain.Aggregates.Users.Services;

namespace Pulse.App.Handlers.Users.Commands.SignIn;

public sealed record SignInUserCommand : ICommand<ErrorOr<AuthDto>>
{
    public string? Username { get; init; }

    public string? Password { get; init; }
}

public sealed class SignInUserCommandAuthorizer : AnonymousAuthorizer<SignInUserCommand>;

public sealed class SignInUserCommandHandler : ICommandHandler<SignInUserCommand, ErrorOr<AuthDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserPasswordHasher _passwordHasher;
    private readonly IJwtProvider _jwtProvider;

    public SignInUserCommandHandler(IUserRepository userRepository,
        IUserPasswordHasher passwordHasher, IJwtProvider jwtProvider)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtProvider = jwtProvider;
    }

    public async Task<ErrorOr<AuthDto>> Handle(SignInUserCommand command, CancellationToken cancellationToken)
    {
        // Find user
        var user = await _userRepository.SearchOne(
            UserByUsernameSpecification.New(command.Username), cancellationToken);

        if (user == null)
        {
            return ApplicationErrors.User.InvalidCredentials;
        }

        // Validate password
        if (!user.IsMatchingPassword(command.Password, _passwordHasher))
        {
            return ApplicationErrors.User.InvalidCredentials;
        }

        // Sign in
        var accessToken = _jwtProvider.Create(user);

        return new AuthDto
        {
            AccessToken = accessToken,
            User = user.ToDto()
        };
    }
}