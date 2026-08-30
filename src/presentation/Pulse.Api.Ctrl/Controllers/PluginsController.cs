using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pulse.Api.Ctrl.Contract;
using Pulse.Api.Ctrl.Controllers.Base;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Dto.Common;
using Pulse.App.Dto.Plugins;
using Pulse.App.Handlers.Plugins.Commands;
using Pulse.App.Handlers.Plugins.Queries;

namespace Pulse.Api.Ctrl.Controllers;

[Route("api/ctrl/v1/plugins")]
public sealed class PluginsController : CtrlApiController
{
    private readonly ISender _sender;

    public PluginsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost(ActionSearch)]
    [ProducesResponseType(typeof(SearchResultDto<PluginDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search(CancellationToken cancellationToken = default)
    {
        var query = new SearchPluginsQuery();
        var result = await _sender.Send(query, cancellationToken);
        return result.Match(Ok, Problem);
    }

    [HttpPost(ActionFetch)]
    [ProducesResponseType(typeof(PluginDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Fetch(
        [FromBody] FetchPluginRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = new FetchPluginQuery
        {
            PluginId = request.PluginId
        };

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(Ok, Problem);
    }

    [HttpPost("verify-connect")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> VerifyConnect(
        [FromBody] VerifyConnectPluginRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new VerifyConnectPluginCommand
        {
            PluginId = request.PluginId,
            Parameters = request.Parameters ?? new Dictionary<string, string>()
        };

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(_ => Ok(), Problem);
    }

    [HttpPost("verify-invoke")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> VerifyConnect(
        [FromBody] VerifyInvokePluginRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new VerifyInvokePluginCommand
        {
            PluginId = request.PluginId,
            Parameters = request.Parameters ?? new Dictionary<string, string>()
        };

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(_ => Ok(), Problem);
    }
}