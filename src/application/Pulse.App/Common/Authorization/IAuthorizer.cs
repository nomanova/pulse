using System.Collections.Generic;

namespace Pulse.App.Common.Authorization;

public interface IAuthorizer<in T>
{
    IEnumerable<IAuthorizationRequirement> Requirements { get; }
    
    void BuildPolicy(T instance);
}