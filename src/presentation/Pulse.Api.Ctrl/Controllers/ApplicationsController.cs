using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pulse.Api.Ctrl.Contract;
using Pulse.Api.Ctrl.Controllers.Base;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Common.Requests;
using Pulse.App.Dto.Applications;
using Pulse.App.Dto.Common;
using Pulse.App.Handlers.Applications.Commands;
using Pulse.App.Handlers.Applications.Queries;
using Pulse.Domain.Aggregates.Applications;
using Pulse.Domain.Aggregates.Organizations;
using Pulse.Domain.Common.Models.Entities;

namespace Pulse.Api.Ctrl.Controllers;

[Route("api/ctrl/v1/applications")]
public sealed class ApplicationsController : CtrlApiController
{
    private readonly ISender _sender;

    public ApplicationsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost(ActionAdd)]
    [ProducesResponseType(typeof(IdentityDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Add(
        [FromBody] AddApplicationRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new AddApplicationCommand
        {
            OrganizationId = request.OrganizationId.AsIdentity<OrganizationId>(),
            ApplicationName = request.ApplicationName
        };

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(Ok, Problem);
    }

    [HttpPost(ActionFetch)]
    [ProducesResponseType(typeof(ApplicationDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Fetch(
        [FromBody] FetchApplicationRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = new FetchApplicationQuery
        {
            ApplicationId = request.ApplicationId.AsIdentity<ApplicationId>()
        };

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(Ok, Problem);
    }

    [HttpPost(ActionSearch)]
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
            OrganizationId = request.OrganizationId.AsIdentity<OrganizationId>()
        };

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(Ok, Problem);
    }

    [HttpPost(ActionRemove)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Remove(
        [FromBody] RemoveApplicationRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new RemoveApplicationCommand
        {
            ApplicationId = request.ApplicationId.AsIdentity<ApplicationId>()
        };

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(_ => Ok(), Problem);
    }
}