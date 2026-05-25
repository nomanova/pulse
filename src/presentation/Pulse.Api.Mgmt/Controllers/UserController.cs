using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pulse.Api.Mgmt.Contract.Users;
using Pulse.Api.Mgmt.Controllers.Base;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Dto.Users;
using Pulse.App.Handlers.Users.Commands.SignIn;

namespace Pulse.Api.Mgmt.Controllers;

[Route("mgmt/v1/users")]
public class UserController : MgmtApiController
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