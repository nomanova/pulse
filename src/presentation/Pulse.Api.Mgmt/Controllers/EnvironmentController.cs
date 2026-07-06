using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pulse.Api.Mgmt.Contract.Environments;
using Pulse.Api.Mgmt.Controllers.Base;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Handlers.Environments.Commands;
using Pulse.Domain.Aggregates.Applications;
using Pulse.Domain.Aggregates.Organizations;
using Pulse.Domain.Common.Models.Entities;

namespace Pulse.Api.Mgmt.Controllers;

[Route("mgmt/v1/environments")]
public class EnvironmentController : MgmtApiController
{
    private readonly ISender _sender;
    
    public EnvironmentController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
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