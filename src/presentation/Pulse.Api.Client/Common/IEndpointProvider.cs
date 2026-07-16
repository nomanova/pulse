using System.Threading.Tasks;

namespace Pulse.Api.Client.Common;

public interface IEndpointProvider
{
    Task<string?> Get();
}