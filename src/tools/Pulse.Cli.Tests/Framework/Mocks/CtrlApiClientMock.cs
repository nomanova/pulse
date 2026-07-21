using Moq;
using Pulse.Api.Ctrl.Client;
using Pulse.Api.Ctrl.Client.Services.Interfaces;

namespace Pulse.Cli.Tests.Framework.Mocks;

public sealed class CtrlApiClientMock : ICtrlApiClient
{
    public CtrlApiClientMock()
    {
        UsersMock = new Mock<IUsersService>();
        OrganizationsMock = new Mock<IOrganizationsService>();
        ApplicationsMock = new Mock<IApplicationsService>();

        Users = UsersMock.Object;
        Organizations = OrganizationsMock.Object;
        Applications = ApplicationsMock.Object;
    }

    public Mock<IUsersService> UsersMock { get; }
    public Mock<IOrganizationsService> OrganizationsMock { get; }
    public Mock<IApplicationsService> ApplicationsMock { get; }

    public IUsersService Users { get; }
    public IOrganizationsService Organizations { get; }
    public IApplicationsService Applications { get; }
}