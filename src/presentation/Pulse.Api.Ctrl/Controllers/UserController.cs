using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pulse.Api.Ctrl.Controllers.Base;
using Pulse.Api.Ctrl.Contract.Users;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Dto.Users;
using Pulse.App.Handlers.Users.Commands;

namespace Pulse.Api.Ctrl.Controllers;

[Route("api/ctrl/v1/users")]
public class UserController : CtrlApiController
{
    private readonly ISender _sender;
    
    public UserController(ISender sender)
    {
        _sender = sender;
    }
    
    [HttpPost]
    [AllowAnonymous]
    [Route("sign-in")]
    [ProducesResponseType(typeof(AuthDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> SignIn([FromBody] SignInRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new SignInUserCommand
        {
            Username = request.Username,
            Password = request.Password
        };

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(Ok, Problem);
    }
}