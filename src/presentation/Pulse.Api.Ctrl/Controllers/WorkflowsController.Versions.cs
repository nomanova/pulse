using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pulse.Api.Ctrl.Contract;
using Pulse.App.Common.Requests;
using Pulse.App.Dto.Common;
using Pulse.App.Dto.Workflows;
using Pulse.App.Handlers.Workflows.Commands.Versions;
using Pulse.App.Handlers.Workflows.Queries.Versions;
using Pulse.Domain.Aggregates.Workflows;
using Pulse.Domain.Aggregates.Workflows.Entities;
using Pulse.Domain.Aggregates.Workflows.Enums;
using Pulse.Domain.Common.Models.Entities;

namespace Pulse.Api.Ctrl.Controllers;

public partial class WorkflowsController
{
    [HttpPost("versions/fetch")]
    [ProducesResponseType(typeof(WorkflowVersionDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> FetchVersion(
        [FromBody] FetchWorkflowVersionRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new FetchWorkflowVersionQuery
        {
            WorkflowVersionId = request.WorkflowVersionId.AsIdentity<WorkflowVersionId>()
        };

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(Ok, Problem);
    }

    [HttpPost("versions/search")]
    [ProducesResponseType(typeof(PagedSearchResultDto<WorkflowVersionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchVersions(
        [FromBody] SearchWorkflowVersionsRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = new SearchWorkflowVersionsQuery
        {
            LastId = request.LastId,
            PageSize = request.PageSize ?? ISearchQuery.DefaultPageSize,
            Ascending = request.Ascending,
            WorkflowId = request.WorkflowId.AsIdentity<WorkflowId>(),
            Status = (WorkflowVersionStatus?)request.Status
        };

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(Ok, Problem);
    }


    [HttpPost("versions/add-step")]
    [ProducesResponseType(typeof(WorkflowVersionStepDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> AddVersionStep(
        [FromBody] AddWorkflowVersionStepRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new AddWorkflowVersionStepCommand
        {
            WorkflowId = request.WorkflowId.AsIdentity<WorkflowId>(),
            VersionId = request.WorkflowVersionId.AsIdentity<WorkflowVersionId>()
        };

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(Ok, Problem);
    }

    [HttpPost("versions/remove-step")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RemoveVersionStep(
        [FromBody] RemoveWorkflowVersionStepRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new RemoveWorkflowVersionStepCommand
        {
            WorkflowId = request.WorkflowId.AsIdentity<WorkflowId>(),
            VersionId = request.WorkflowVersionId.AsIdentity<WorkflowVersionId>(),
            StepId = request.WorkflowVersionStepId.AsIdentity<WorkflowVersionStepId>()
        };

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(_ => Ok(), Problem);
    }
}