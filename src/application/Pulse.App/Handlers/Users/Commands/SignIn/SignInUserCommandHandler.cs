using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Dto.Users;

namespace Pulse.App.Handlers.Users.Commands.SignIn;

public sealed class SignInUserCommandHandler : ICommandHandler<SignInUserCommand, ErrorOr<ProfileDto>>
{
    public ValueTask<ErrorOr<ProfileDto>> Handle(SignInUserCommand command, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }
}