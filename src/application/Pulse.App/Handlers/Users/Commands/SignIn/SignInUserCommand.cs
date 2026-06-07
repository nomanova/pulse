using ErrorOr;
using Pulse.App.Common.Authorization.Policies;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Dto.Users;

namespace Pulse.App.Handlers.Users.Commands.SignIn;

public sealed record SignInUserCommand : ICommand<ErrorOr<AuthDto>>
{
    public string? Username { get; init; }

    public string? Password { get; init; }
}

public sealed class SignInUserCommandAuthorizer : AnonymousAuthorizer<SignInUserCommand>;