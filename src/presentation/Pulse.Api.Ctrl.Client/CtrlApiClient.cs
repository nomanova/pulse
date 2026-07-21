using Pulse.Api.Client;
using Pulse.Api.Ctrl.Client.Services;
using Pulse.Api.Ctrl.Client.Services.Interfaces;

namespace Pulse.Api.Ctrl.Client;

public class CtrlApiClient : ApiClient, ICtrlApiClient
{
    public IUsersService Users { get; private set; } = null!;
    
    public IOrganizationsService Organizations { get; private set; } = null!;
    
    public IApplicationsService Applications { get; private set; } = null!;
    
    public CtrlApiClient(ApiClientOptions options) : base(options)
    {
        CreateServices(options);
    }

    private void CreateServices(ApiClientOptions options)
    {
        Users = new UsersService(options.EndpointProvider, options.TokenProvider, HttpClient);
        Organizations = new OrganizationsService(options.EndpointProvider, options.TokenProvider, HttpClient);
        Applications = new ApplicationsService(options.EndpointProvider, options.TokenProvider, HttpClient);
    }
}