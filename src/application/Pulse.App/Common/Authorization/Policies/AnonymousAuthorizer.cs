namespace Pulse.App.Common.Authorization.Policies;

/**
 * User must not be authenticated, zero requirements.
 */
public abstract class AnonymousAuthorizer<TRequest> : Authorizer<TRequest>
{
    public override void BuildPolicy(TRequest request)
    {
        // No requirements - marker authorizer
    }
}