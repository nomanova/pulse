using Microsoft.AspNetCore.Authentication;

namespace Pulse.Infra.Security.Authentication.ApiKey;

public sealed class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    public const string HeaderName = "X-Api-Key";
}
