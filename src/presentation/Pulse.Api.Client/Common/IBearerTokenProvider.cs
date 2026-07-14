using System.Threading.Tasks;

namespace Pulse.Api.Client.Common;

public interface IBearerTokenProvider
{
    Task<string?> GetToken();
}