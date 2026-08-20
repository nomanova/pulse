using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Pulse.Api.Client.Common;

namespace Pulse.Api.Client.Handlers;

internal class TokenRequestHandler : DelegatingHandler
{
    private readonly Func<ITokenProvider> _tokenProviderFunc;

    public TokenRequestHandler(Func<ITokenProvider> tokenProviderFunc)
    {
        _tokenProviderFunc = tokenProviderFunc;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var tokenProvider = _tokenProviderFunc();
        ArgumentNullException.ThrowIfNull(tokenProvider);

        var bearerToken = await tokenProvider.Get();
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

        return await base.SendAsync(request, cancellationToken);
    }
}