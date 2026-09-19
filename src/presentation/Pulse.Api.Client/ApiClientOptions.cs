using System;
using Pulse.Api.Client.Common;
using Pulse.Api.Client.Handlers;

namespace Pulse.Api.Client;

public sealed record ApiClientOptions
{
    public IEndpointProvider? EndpointProvider { get; init; }

    public TimeSpan? RequestTimeout { get; init; }

    public ITokenProvider TokenProvider { get; init; }

    public IResponseHandler ResponseHandler { get; init; }
}