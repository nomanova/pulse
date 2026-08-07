using Pulse.Api.Client;
using Pulse.Api.Data.Client.Services;
using Pulse.Api.Data.Client.Services.Interfaces;

namespace Pulse.Api.Data.Client;

public sealed class DataApiClient : ApiClient, IDataApiClient
{
    public IWorkflowsService Workflows { get; private set; } = null!;

    public DataApiClient(ApiClientOptions options) : base(options)
    {
        CreateServices(options);
    }

    private void CreateServices(ApiClientOptions options)
    {
        Workflows = new WorkflowsService(options.EndpointProvider, options.TokenProvider, HttpClient);
    }
}