using Pulse.Api.Client;
using Pulse.Api.Client.Common;
using Pulse.Api.Ctrl.Client.Applications;
using Pulse.Api.Ctrl.Client.Users;

namespace Pulse.Api.Ctrl.Client;

public class CtrlApiClient : ApiClient, ICtrlApiClient
{
    public IUsersService Users { get; private set; } = null!;
    
    public IApplicationsService Applications { get; private set; } = null!;
    
    public CtrlApiClient(ApiClientOptions options) : base(options)
    {
        CreateServices(options.ApiEndpointProvider);
    }

    private void CreateServices(IEndpointProvider? endpointProvider = null)
    {
        Users = new UsersService(endpointProvider, HttpClient);
        Applications = new ApplicationsService(endpointProvider, HttpClient);
    }
}