using System.Threading.Tasks;

namespace Pulse.Api.Client.Common;

public interface IApiEndpointProvider
{
    Task<string?> GetApiEndpoint();
}