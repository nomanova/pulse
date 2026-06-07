using Pulse.App.Common.Authorization;

namespace Pulse.App.Common.Security.Interfaces;

public interface IResourceAuthorizationContextProvider
{
    ResourceAuthorizationContext Context { get; set; }
}