using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pulse.Api.Ctrl.Contract.Organizations;
using Pulse.Api.Ctrl.Controllers.Base;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Dto.Common;
using Pulse.App.Handlers.Organizations.Commands;

namespace Pulse.Api.Ctrl.Controllers;

[Route("api/ctrl/v1/organizations")]
public class OrganizationController : CtrlApiController
{
    private readonly ISender _sender;

    public OrganizationController(ISender sender)
    {
        _sender = sender;
    }
    
    [HttpPost("create")]
    [ProducesResponseType(typeof(IdentityDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Create(
        [FromBody] CreateOrganizationRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateOrganizationCommand
        {
            OrganizationName = request.OrganizationName
        };

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(Ok, Problem);
    }
}