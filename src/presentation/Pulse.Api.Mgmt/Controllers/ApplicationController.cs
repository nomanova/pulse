using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pulse.Api.Mgmt.Contract.Applications;
using Pulse.Api.Mgmt.Controllers.Base;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Handlers.Applications.Commands;

namespace Pulse.Api.Mgmt.Controllers;

[Route("mgmt/v1/applications")]
public class ApplicationController : MgmtApiController
{
    private readonly ISender _sender;
    
    public ApplicationController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Create(
        [FromBody] CreateApplicationRequest request, 
        CancellationToken cancellationToken = default)
    {
        var command = new CreateApplicationCommand
        {
            OrganizationId = request.OrganizationId,
            Name = request.Name
        };
        
        var result = await _sender.Send(command, cancellationToken);
        
        return result.Match(Ok, Problem);
    }
}