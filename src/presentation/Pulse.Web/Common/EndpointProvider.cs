using System.Threading.Tasks;
using Pulse.Api.Client.Common;

namespace Pulse.Web.Common;

public sealed class EndpointProvider : IEndpointProvider
{
    private readonly string? _endpoint;
    
    public EndpointProvider(string? endpoint)
    {
        _endpoint = endpoint;
    }

    public Task<string?> Get()
    {
        return Task.FromResult(_endpoint);
    }
}