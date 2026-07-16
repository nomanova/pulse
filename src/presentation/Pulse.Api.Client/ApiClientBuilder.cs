using System;
using Pulse.Api.Client.Common;

namespace Pulse.Api.Client;

public abstract class ApiClientBuilder<T> where T : ApiClient
{
    protected IEndpointProvider? ApiEndpointProvider;
    protected TimeSpan? RequestTimeout;
    protected Func<IBearerTokenProvider>? BearerTokenProvider;

    public ApiClientBuilder<T> WithApiEndpoint(IEndpointProvider provider)
    {
        ApiEndpointProvider = provider;
        return this;
    }

    public ApiClientBuilder<T> WithRequestTimeout(TimeSpan requestTimeout)
    {
        RequestTimeout = requestTimeout;
        return this;
    }

    public ApiClientBuilder<T> WithBearerToken(Func<IBearerTokenProvider> provider)
    {
        BearerTokenProvider = provider;
        return this;
    }

    public abstract T Build();
}