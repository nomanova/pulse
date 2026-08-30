using Pulse.App.Common.Authorization.Requirements;

namespace Pulse.App.Common.Authorization.Policies;

public abstract class ApiKeyAuthorizer<TRequest> : Authorizer<TRequest>
{
    public override void BuildPolicy(TRequest request)
    {
        UseRequirement(new MustHaveValidApiKeyRequirement());
    }
}