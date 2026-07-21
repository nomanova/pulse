using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pulse.Api.Ctrl.Contract.Organizations;
using Pulse.Api.Ctrl.Controllers.Base;
using Pulse.Api.Shared.Contract;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Common.Requests;
using Pulse.App.Dto.Common;
using Pulse.App.Dto.Organizations;
using Pulse.App.Handlers.Organizations.Commands;
using Pulse.App.Handlers.Organizations.Queries;

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

    [HttpPost("fetch")]
    [ProducesResponseType(typeof(OrganizationDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Fetch(
        [FromBody] FetchOrganizationRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = new FetchOrganizationQuery
        {
            OrganizationName = request.OrganizationName
        };

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(Ok, Problem);
    }

    [HttpPost("search")]
    [ProducesResponseType(typeof(PagedSearchResultDto<OrganizationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search(
        [FromBody] PagedSearchRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = new SearchOrganizationsQuery
        {
            Query = request.Query,
            LastId = request.LastId,
            PageSize = request.PageSize ?? ISearchQuery.DefaultPageSize,
            Ascending = request.Ascending,
            OrderBy = request.OrderBy
        };

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(Ok, Problem);
    }

    [HttpPost("delete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete(
        [FromBody] DeleteOrganizationRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteOrganizationCommand
        {
            OrganizationName = request.OrganizationName
        };

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(_ => Ok(), Problem);
    }
}