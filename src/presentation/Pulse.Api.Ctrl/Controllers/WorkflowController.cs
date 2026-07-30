using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pulse.Api.Ctrl.Contract.Workflows;
using Pulse.Api.Ctrl.Controllers.Base;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Common.Requests;
using Pulse.App.Dto.Common;
using Pulse.App.Dto.Workflows;
using Pulse.App.Handlers.Workflows.Commands;
using Pulse.App.Handlers.Workflows.Commands.Steps;
using Pulse.App.Handlers.Workflows.Queries;

namespace Pulse.Api.Ctrl.Controllers;

[Route("api/ctrl/v1/workflows")]
public sealed class WorkflowController : CtrlApiController
{
    private readonly ISender _sender;

    public WorkflowController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("create")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Create(
        [FromBody] CreateWorkflowRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateWorkflowCommand
        {
            OrganizationName = request.OrganizationName,
            ApplicationName = request.ApplicationName,
            EnvironmentName = request.EnvironmentName,
            WorkflowName = request.WorkflowName
        };

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(Ok, Problem);
    }

    [HttpPost("fetch")]
    [ProducesResponseType(typeof(WorkflowDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Fetch(
        [FromBody] FetchWorkflowRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = new FetchWorkflowQuery
        {
            OrganizationName = request.OrganizationName,
            ApplicationName = request.ApplicationName,
            EnvironmentName = request.EnvironmentName,
            WorkflowName = request.WorkflowName
        };

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(Ok, Problem);
    }

    [HttpPost("search")]
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
            OrganizationName = request.OrganizationName,
            ApplicationName = request.ApplicationName,
            EnvironmentName = request.EnvironmentName
        };

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(Ok, Problem);
    }

    [HttpPost("delete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete(
        [FromBody] DeleteWorkflowRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteWorkflowCommand
        {
            OrganizationName = request.OrganizationName,
            ApplicationName = request.ApplicationName,
            EnvironmentName = request.EnvironmentName,
            WorkflowName = request.WorkflowName
        };

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(_ => Ok(), Problem);
    }

    [HttpPost("add-step")]
    [ProducesResponseType(typeof(WorkflowVersionStepDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> AddStep(
        [FromBody] AddWorkflowStepRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new AddWorkflowStepCommand
        {
            OrganizationName = request.OrganizationName,
            ApplicationName = request.ApplicationName,
            EnvironmentName = request.EnvironmentName,
            WorkflowName = request.WorkflowName
        };

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(Ok, Problem);
    }
}