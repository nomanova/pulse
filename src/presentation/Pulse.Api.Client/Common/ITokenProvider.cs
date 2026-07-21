using System.Threading.Tasks;

namespace Pulse.Api.Client.Common;

public interface ITokenProvider
{
    Task<string?> Get();
}