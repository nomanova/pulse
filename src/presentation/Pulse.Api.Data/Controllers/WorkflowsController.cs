using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pulse.Api.Data.Contract;
using Pulse.Api.Data.Controllers.Base;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Common.Requests;
using Pulse.App.Dto.Common;
using Pulse.App.Dto.Workflows;
using Pulse.App.Handlers.Workflows.Commands;
using Pulse.App.Handlers.Workflows.Queries;
using Pulse.Domain.Aggregates.Environments;
using Pulse.Domain.Aggregates.Workflows;
using Pulse.Domain.Common.Models.Entities;

namespace Pulse.Api.Data.Controllers;

[Route("api/data/v1/workflows")]
public partial class WorkflowsController : DataApiController
{
    private readonly ISender _sender;

    public WorkflowsController(ISender sender)
    {
        _sender = sender;
    }
    
    [HttpPost(ActionAdd)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Add(
        [FromBody] AddWorkflowRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new AddWorkflowCommand
        {
            EnvironmentId = request.EnvironmentId.AsIdentity<EnvironmentId>(),
            WorkflowName = request.WorkflowName
        };

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(Ok, Problem);
    }

    [HttpPost(ActionFetch)]
    [ProducesResponseType(typeof(WorkflowDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Fetch(
        [FromBody] FetchWorkflowRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = new FetchWorkflowQuery
        {
            WorkflowId = request.WorkflowId.AsIdentity<WorkflowId>()
        };

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(Ok, Problem);
    }

    [HttpPost(ActionSearch)]
    [ProducesResponseType(typeof(PagedSearchResultDto<WorkflowDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search(
        [FromBody] SearchWorkflowsRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = new SearchWorkflowsQuery
        {
            Query = request.Query,
            LastId = request.LastId,
            PageSize = request.PageSize ?? ISearchQuery.DefaultPageSize,
            Ascending = request.Ascending,
            OrderBy = request.OrderBy,
            EnvironmentId = request.EnvironmentId.AsIdentity<EnvironmentId>()
        };

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(Ok, Problem);
    }

    [HttpPost(ActionRemove)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Remove(
        [FromBody] RemoveWorkflowRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new RemoveWorkflowCommand
        {
            WorkflowId = request.WorkflowId.AsIdentity<WorkflowId>()
        };

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(_ => Ok(), Problem);
    }

    [HttpPost("publish")]
    [ProducesResponseType(typeof(PagedSearchResultDto<WorkflowVersionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Publish(
        [FromBody] PublishWorkflowRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new PublishWorkflowCommand
        {
            WorkflowId = request.WorkflowId.AsIdentity<WorkflowId>()
        };

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(Ok, Problem);
    }
}