using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pulse.Api.Ctrl.Contract;
using Pulse.App.Dto.Workflows;
using Pulse.App.Handlers.Workflows.Commands.Versions;
using Pulse.Domain.Aggregates.Applications;
using Pulse.Domain.Aggregates.Environments;
using Pulse.Domain.Aggregates.Organizations;
using Pulse.Domain.Aggregates.Workflows;
using Pulse.Domain.Aggregates.Workflows.Entities;
using Pulse.Domain.Common.Models.Entities;

namespace Pulse.Api.Ctrl.Controllers;

public partial class WorkflowsController
{
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