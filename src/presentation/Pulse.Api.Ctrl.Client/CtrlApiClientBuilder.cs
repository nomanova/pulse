using Pulse.Api.Client;

namespace Pulse.Api.Ctrl.Client;

public sealed class CtrlApiClientBuilder : ApiClientBuilder<CtrlApiClient>
{
    public override CtrlApiClient Build()
    {
        var clientOptions = new ApiClientOptions
        {
            EndpointProvider = EndpointProvider,
            TokenProvider = TokenProvider,
            RequestTimeout = RequestTimeout
        };
        
        return new CtrlApiClient(clientOptions);
    }
}