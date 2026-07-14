using System.Net.Http;

namespace Pulse.Api.Client.Handlers;

public static class HandlerExtensions
{
    public static DelegatingHandler DecorateWith(this HttpMessageHandler clientHandler, DelegatingHandler outerHandler)
    {
        outerHandler.InnerHandler = clientHandler;
        return outerHandler;
    }
}