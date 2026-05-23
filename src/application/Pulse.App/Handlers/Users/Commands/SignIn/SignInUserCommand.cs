using ErrorOr;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Dto.Users;

namespace Pulse.App.Handlers.Users.Commands.SignIn;

public sealed record SignInUserCommand : ICommand<ErrorOr<ProfileDto>>
{
    public string? EmailAddress { get; init; }

    public string? Password { get; init; }
}