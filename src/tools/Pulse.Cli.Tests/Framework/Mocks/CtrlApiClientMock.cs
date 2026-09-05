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
        EnvironmentsMock = new Mock<IEnvironmentsService>();
        ConnectionsMock = new Mock<IConnectionsService>();
        
        Users = UsersMock.Object;
        Organizations = OrganizationsMock.Object;
        Applications = ApplicationsMock.Object;
        Environments = EnvironmentsMock.Object;
        Connections = ConnectionsMock.Object;
    }

    public Mock<IUsersService> UsersMock { get; }
    public Mock<IOrganizationsService> OrganizationsMock { get; }
    public Mock<IApplicationsService> ApplicationsMock { get; }
    public Mock<IEnvironmentsService> EnvironmentsMock { get; }
    public Mock<IConnectionsService> ConnectionsMock { get; }

    public IUsersService Users { get; }
    public IOrganizationsService Organizations { get; }
    public IApplicationsService Applications { get; }
    public IEnvironmentsService Environments { get; }
    public IConnectionsService Connections { get; }
}