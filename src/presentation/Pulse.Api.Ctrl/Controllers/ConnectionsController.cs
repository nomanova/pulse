using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pulse.Api.Ctrl.Contract;
using Pulse.Api.Ctrl.Controllers.Base;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Dto.Common;
using Pulse.App.Handlers.Connections.Commands;
using Pulse.Domain.Aggregates.Environments;
using Pulse.Domain.Common.Models.Entities;

namespace Pulse.Api.Ctrl.Controllers;

[Route("api/ctrl/v1/connections")]
public sealed class ConnectionsController : CtrlApiController
{
    private readonly ISender _sender;

    public ConnectionsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost(ActionAdd)]
    [ProducesResponseType(typeof(IdentityDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Add(
        [FromBody] AddConnectionRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new AddConnectionCommand
        {
            EnvironmentId = request.EnvironmentId.AsIdentity<EnvironmentId>(),
            PluginId = request.PluginId,
            Parameters = request.Parameters ?? new Dictionary<string, string>()
        };

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(Ok, Problem);
    }
}