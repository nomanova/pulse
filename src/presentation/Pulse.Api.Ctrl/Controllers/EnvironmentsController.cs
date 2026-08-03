using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pulse.Api.Ctrl.Contract;
using Pulse.Api.Ctrl.Controllers.Base;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Common.Requests;
using Pulse.App.Dto.Common;
using Pulse.App.Dto.Environments;
using Pulse.App.Handlers.Environments.Commands;
using Pulse.App.Handlers.Environments.Queries;
using Pulse.Domain.Aggregates.Applications;
using Pulse.Domain.Aggregates.Environments;
using Pulse.Domain.Aggregates.Organizations;
using Pulse.Domain.Common.Models.Entities;

namespace Pulse.Api.Ctrl.Controllers;

[Route("api/ctrl/v1/environments")]
public sealed class EnvironmentsController : CtrlApiController
{
    private readonly ISender _sender;

    public EnvironmentsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost(ActionAdd)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Add(
        [FromBody] AddEnvironmentRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new AddEnvironmentCommand
        {
            ApplicationId = request.ApplicationId.AsIdentity<ApplicationId>(),
            EnvironmentName = request.EnvironmentName
        };

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(Ok, Problem);
    }

    [HttpPost(ActionFetch)]
    [ProducesResponseType(typeof(EnvironmentDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Fetch(
        [FromBody] FetchEnvironmentRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = new FetchEnvironmentQuery
        {
            EnvironmentId = request.EnvironmentId.AsIdentity<EnvironmentId>()
        };

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(Ok, Problem);
    }

    [HttpPost(ActionSearch)]
    [ProducesResponseType(typeof(PagedSearchResultDto<EnvironmentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search(
        [FromBody] SearchEnvironmentsRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = new SearchEnvironmentsQuery
        {
            Query = request.Query,
            LastId = request.LastId,
            PageSize = request.PageSize ?? ISearchQuery.DefaultPageSize,
            Ascending = request.Ascending,
            OrderBy = request.OrderBy,
            ApplicationId = request.ApplicationId.AsIdentity<ApplicationId>()
        };

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(Ok, Problem);
    }

    [HttpPost(ActionRemove)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Remove(
        [FromBody] RemoveEnvironmentRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new RemoveEnvironmentCommand
        {
            EnvironmentId = request.EnvironmentId.AsIdentity<EnvironmentId>()
        };

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(_ => Ok(), Problem);
    }
}