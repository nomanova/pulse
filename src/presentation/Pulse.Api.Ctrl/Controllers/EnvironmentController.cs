using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pulse.Api.Ctrl.Controllers.Base;
using Pulse.Api.Ctrl.Contract.Environments;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Handlers.Environments.Commands;

namespace Pulse.Api.Ctrl.Controllers;

[Route("api/ctrl/v1/environments")]
public class EnvironmentController : CtrlApiController
{
    private readonly ISender _sender;
    
    public EnvironmentController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("create")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Create(
        [FromBody] CreateEnvironmentRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateEnvironmentCommand
        {
            OrganizationName = request.OrganizationName,
            ApplicationName = request.ApplicationName,
            EnvironmentName = request.EnvironmentName
        };
        
        var result = await _sender.Send(command, cancellationToken);
        
        return result.Match(Ok, Problem);
    }
}