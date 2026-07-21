using Pulse.Api.Ctrl.Client.Services.Interfaces;

namespace Pulse.Api.Ctrl.Client;

public interface ICtrlApiClient
{
    IUsersService Users { get; }
    
    IOrganizationsService Organizations { get; }
    
    IApplicationsService Applications { get; }
}