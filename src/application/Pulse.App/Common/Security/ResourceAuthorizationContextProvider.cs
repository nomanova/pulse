using Pulse.App.Common.Authorization;
using Pulse.App.Common.Exceptions;
using Pulse.App.Common.Security.Interfaces;

namespace Pulse.App.Common.Security;

public sealed class ResourceAuthorizationContextProvider : IResourceAuthorizationContextProvider
{
    public ResourceAuthorizationContext Context
    {
        get => field ?? throw new UnauthorizedException();
        set;
    }
}