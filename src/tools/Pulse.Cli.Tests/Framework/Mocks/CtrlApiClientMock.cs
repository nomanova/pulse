using Moq;
using Pulse.Api.Ctrl.Client;
using Pulse.Api.Ctrl.Client.Users;

namespace Pulse.Cli.Tests.Framework.Mocks;

public sealed class CtrlApiClientMock : ICtrlApiClient
{
    public CtrlApiClientMock()
    {
        UsersMock = new Mock<IUsersService>();

        Users = UsersMock.Object;
    }

    public Mock<IUsersService> UsersMock { get; }

    public IUsersService Users { get; }
}