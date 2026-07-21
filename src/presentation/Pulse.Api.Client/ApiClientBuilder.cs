using System;
using Pulse.Api.Client.Common;

namespace Pulse.Api.Client;

public abstract class ApiClientBuilder<T> where T : ApiClient
{
    protected IEndpointProvider? EndpointProvider;
    protected TimeSpan? RequestTimeout;
    protected ITokenProvider? TokenProvider;

    public ApiClientBuilder<T> WithEndpoint(IEndpointProvider provider)
    {
        EndpointProvider = provider;
        return this;
    }

    public ApiClientBuilder<T> WithTimeout(TimeSpan requestTimeout)
    {
        RequestTimeout = requestTimeout;
        return this;
    }

    public ApiClientBuilder<T> WithToken(ITokenProvider provider)
    {
        TokenProvider = provider;
        return this;
    }

    public abstract T Build();
}