using System;
using Pulse.Api.Client.Common;

namespace Pulse.Api.Client;

public sealed record ApiClientOptions
{
    public IEndpointProvider? EndpointProvider { get; init; }
    
    public TimeSpan? RequestTimeout { get; init; }
    
    public ITokenProvider? TokenProvider { get; init; }
}
