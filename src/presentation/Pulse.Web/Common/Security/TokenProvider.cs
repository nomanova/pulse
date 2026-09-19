using System.Threading.Tasks;
using Pulse.Api.Client.Common;
using Pulse.Web.Core.Services.Interfaces;

namespace Pulse.Web.Common.Security;

public sealed class TokenProvider : ITokenProvider
{
    private readonly IAuthenticationStore _authenticationStore;

    public TokenProvider(IAuthenticationStore authenticationStore)
    {
        _authenticationStore = authenticationStore;
    }

    public async Task<string?> Get()
    {
        return await _authenticationStore.GetToken();
    }
}