using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pulse.Api.Ctrl.Controllers.Base;
using Pulse.Api.Ctrl.Contract.Environments;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Handlers.Environments.Commands;
using Pulse.Domain.Aggregates.Applications;
using Pulse.Domain.Aggregates.Organizations;
using Pulse.Domain.Common.Models.Entities;

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
            OrganizationId = request.OrganizationId.AsIdentity<OrganizationId>(),
            ApplicationId = request.ApplicationId.AsIdentity<ApplicationId>(),
            Name = request.Name
        };
        
        var result = await _sender.Send(command, cancellationToken);
        
        return result.Match(Ok, Problem);
    }
}