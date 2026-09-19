using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Pulse.Api.Client.Handlers;

internal class UserResponseHandler : DelegatingHandler
{
    private readonly IResponseHandler _handler;

    public UserResponseHandler(IResponseHandler handler)
    {
        _handler = handler;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);

        await _handler.Handle(response);

        return response;
    }
}