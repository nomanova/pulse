using System.Net.Http;
using System.Threading.Tasks;

namespace Pulse.Api.Client.Handlers;

public interface IResponseHandler
{
    Task Handle(HttpResponseMessage message);
}