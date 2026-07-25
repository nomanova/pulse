using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pulse.Api.Ctrl.Controllers.Base;
using Pulse.Api.Ctrl.Contract.Environments;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Common.Requests;
using Pulse.App.Dto.Common;
using Pulse.App.Dto.Environments;
using Pulse.App.Handlers.Environments.Commands;
using Pulse.App.Handlers.Environments.Queries;

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

    [HttpPost("fetch")]
    [ProducesResponseType(typeof(EnvironmentDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Fetch(
        [FromBody] FetchEnvironmentRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = new FetchEnvironmentQuery
        {
            OrganizationName = request.OrganizationName,
            ApplicationName = request.ApplicationName,
            EnvironmentName = request.EnvironmentName
        };

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(Ok, Problem);
    }

    [HttpPost("search")]
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
            OrganizationName = request.OrganizationName,
            ApplicationName = request.ApplicationName
        };

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(Ok, Problem);
    }

    [HttpPost("delete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete(
        [FromBody] DeleteEnvironmentRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteEnvironmentCommand
        {
            OrganizationName = request.OrganizationName,
            ApplicationName = request.ApplicationName,
            EnvironmentName = request.EnvironmentName
        };

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(_ => Ok(), Problem);
    }
}