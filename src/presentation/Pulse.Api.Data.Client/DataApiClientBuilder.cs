using Pulse.Api.Client;

namespace Pulse.Api.Data.Client;

public sealed class DataApiClientBuilder : ApiClientBuilder<DataApiClient>
{
    public override DataApiClient Build()
    {
        var clientOptions = new ApiClientOptions
        {
            EndpointProvider = EndpointProvider,
            TokenProvider = TokenProvider,
            RequestTimeout = RequestTimeout
        };
        
        return new DataApiClient(clientOptions);
    }
}