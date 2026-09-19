using System;
using Pulse.Api.Client.Common;
using Pulse.Api.Client.Handlers;

namespace Pulse.Api.Client;

public abstract class ApiClientBuilder<T> where T : ApiClient
{
    protected IEndpointProvider? EndpointProvider;
    protected TimeSpan? RequestTimeout;
    protected ITokenProvider? TokenProvider;
    protected IResponseHandler? ResponseHandler;

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

    public ApiClientBuilder<T> WithTokenProvider(ITokenProvider provider)
    {
        TokenProvider = provider;
        return this;
    }
    
    public ApiClientBuilder<T> WithResponseHandler(IResponseHandler handler)
    {
        ResponseHandler = handler;
        return this;
    }

    public abstract T Build();
}