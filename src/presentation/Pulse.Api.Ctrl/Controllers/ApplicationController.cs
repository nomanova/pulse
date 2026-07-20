using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pulse.Api.Ctrl.Controllers.Base;
using Pulse.Api.Ctrl.Contract.Applications;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Common.Requests;
using Pulse.App.Dto.Applications;
using Pulse.App.Dto.Common;
using Pulse.App.Handlers.Applications.Commands;
using Pulse.App.Handlers.Applications.Queries;

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
    [ProducesResponseType(typeof(IdentityDto), StatusCodes.Status200OK)]
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

    [HttpPost("fetch")]
    [ProducesResponseType(typeof(ApplicationDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Fetch(
        [FromBody] FetchApplicationRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = new FetchApplicationQuery
        {
            OrganizationName = request.OrganizationName,
            ApplicationName = request.ApplicationName
        };
        
        var result = await _sender.Send(query, cancellationToken);
        
        return result.Match(Ok, Problem);
    }

    [HttpPost("search")]
    [ProducesResponseType(typeof(PagedSearchResultDto<ApplicationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search(
        [FromBody] SearchApplicationsRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = new SearchApplicationsQuery
        {
            Query = request.Query,
            LastId = request.LastId,
            PageSize = request.PageSize ?? ISearchQuery.DefaultPageSize,
            Ascending = request.Ascending,
            OrderBy = request.OrderBy,
            OrganizationName = request.OrganizationName
        };

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(Ok, Problem);
    }

    [HttpPost("delete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete(
        [FromBody] DeleteApplicationRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteApplicationCommand
        {
            OrganizationName = request.OrganizationName,
            ApplicationName = request.ApplicationName
        };

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(_ => Ok(), Problem);
    }
}