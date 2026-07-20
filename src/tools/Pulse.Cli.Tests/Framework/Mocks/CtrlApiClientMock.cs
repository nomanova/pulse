using Moq;
using Pulse.Api.Ctrl.Client;
using Pulse.Api.Ctrl.Client.Applications;
using Pulse.Api.Ctrl.Client.Users;

namespace Pulse.Cli.Tests.Framework.Mocks;

public sealed class CtrlApiClientMock : ICtrlApiClient
{
    public CtrlApiClientMock()
    {
        UsersMock = new Mock<IUsersService>();
        ApplicationsMock = new Mock<IApplicationsService>();

        Users = UsersMock.Object;
        Applications = ApplicationsMock.Object;
    }

    public Mock<IUsersService> UsersMock { get; }
    public Mock<IApplicationsService> ApplicationsMock { get; }

    public IUsersService Users { get; }
    public IApplicationsService Applications { get; }
}