using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Pulse.Api.Client.Handlers;
using Pulse.Web.Core;
using Pulse.Web.Core.Services.Interfaces;

namespace Pulse.Web.Common.Navigation;

public sealed class GlobalResponseHandler : IResponseHandler
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IPageNavigator _navigator;

    public GlobalResponseHandler(
        IAuthenticationService authenticationService,
        IPageNavigator navigator)
    {
        _authenticationService = authenticationService;
        _navigator = navigator;
    }

    public async Task Handle(HttpResponseMessage message)
    {
        switch (message.StatusCode)
        {
            case HttpStatusCode.Forbidden:
            case HttpStatusCode.Unauthorized:
                await _authenticationService.SignOut();
                _navigator.NavigateTo(Routes.SignIn);
                break;
            case HttpStatusCode.ServiceUnavailable:
                _navigator.NavigateTo(Routes.Error503);
                break;
        }
    }
}