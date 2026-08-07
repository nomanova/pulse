using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pulse.Api.Ctrl.Contract;
using Pulse.Api.Ctrl.Controllers.Base;
using Pulse.Api.Shared.Contract;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Common.Requests;
using Pulse.App.Dto.Common;
using Pulse.App.Dto.Organizations;
using Pulse.App.Handlers.Organizations.Commands;
using Pulse.App.Handlers.Organizations.Queries;
using Pulse.Domain.Aggregates.Organizations;
using Pulse.Domain.Common.Models.Entities;

namespace Pulse.Api.Ctrl.Controllers;

[Route("api/ctrl/v1/organizations")]
public sealed class OrganizationsController : CtrlApiController
{
    private readonly ISender _sender;

    public OrganizationsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost(ActionAdd)]
    [ProducesResponseType(typeof(IdentityDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Add(
        [FromBody] AddOrganizationRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new AddOrganizationCommand
        {
            OrganizationName = request.OrganizationName
        };

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(Ok, Problem);
    }

    [HttpPost(ActionFetch)]
    [ProducesResponseType(typeof(OrganizationDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Fetch(
        [FromBody] FetchOrganizationRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = new FetchOrganizationQuery
        {
            OrganizationId = request.OrganizationId.AsIdentity<OrganizationId>()
        };

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(Ok, Problem);
    }

    [HttpPost(ActionSearch)]
    [ProducesResponseType(typeof(PagedSearchResultDto<OrganizationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search(
        [FromBody] NamedPagedSearchRequest request,
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

    [HttpPost(ActionRemove)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Remove(
        [FromBody] RemoveOrganizationRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new RemoveOrganizationCommand
        {
            OrganizationId = request.OrganizationId.AsIdentity<OrganizationId>()
        };

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(_ => Ok(), Problem);
    }
}