using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pulse.Api.Ctrl.Controllers.Base;
using Pulse.Api.Ctrl.Contract.Applications;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Handlers.Applications.Commands;

namespace Pulse.Api.Ctrl.Controllers;

[Route("api/ctrl/v1/applications")]
public class ApplicationController : CtrlApiController
{
    private readonly ISender _sender;
    
    public ApplicationController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("create")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Create(
        [FromBody] CreateApplicationRequest request, 
        CancellationToken cancellationToken = default)
    {
        var command = new CreateApplicationCommand
        {
            OrganizationName = request.OrganizationName,
            ApplicationName = request.ApplicationName
        };
        
        var result = await _sender.Send(command, cancellationToken);
        
        return result.Match(Ok, Problem);
    }
}