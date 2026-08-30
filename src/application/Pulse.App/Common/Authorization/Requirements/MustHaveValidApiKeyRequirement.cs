using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using Pulse.App.Common.Security.Interfaces;

namespace Pulse.App.Common.Authorization.Requirements;

/**
 * This requirement only checks if the provided API key exists and matches a known value.
 * If it passes, the environment linked to the API key is loaded into the EnvironmentProvider.
 */
public sealed class MustHaveValidApiKeyRequirement : IAuthorizationRequirement;

public class MustHaveValidApiKeyRequirementHandler : IAuthorizationHandler<MustHaveValidApiKeyRequirement>
{
    private readonly IEnvironmentProvider _environmentProvider;
    
    public MustHaveValidApiKeyRequirementHandler(IEnvironmentProvider environmentProvider)
    {
        _environmentProvider = environmentProvider;
    }

    public async Task<ErrorOr<Success>> Handle(
        MustHaveValidApiKeyRequirement request,
        CancellationToken cancellationToken)
    {
        await _environmentProvider.Get(cancellationToken);
        return Result.Success;
    }
}