using System;
using Pulse.Api.Client.Common;

namespace Pulse.Api.Client;

public sealed record ApiClientOptions
{
    public IEndpointProvider? ApiEndpointProvider { get; init; }
    
    public TimeSpan? RequestTimeout { get; init; }
    
    public Func<IBearerTokenProvider>? BearerTokenProvider { get; init; }
}
