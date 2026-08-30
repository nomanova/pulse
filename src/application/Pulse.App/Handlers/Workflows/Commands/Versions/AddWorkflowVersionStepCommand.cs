using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using Pulse.App.Common.Authorization.Policies;
using Pulse.App.Common.Database;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Dto.Common;
using Pulse.App.Dto.Workflows;
using Pulse.App.Handlers.Workflows.Common;
using Pulse.App.Handlers.Workflows.Common.Specifications;
using Pulse.Domain.Aggregates.Workflows;
using Pulse.Domain.Aggregates.Workflows.Entities;
using Pulse.Domain.Aggregates.Workflows.ValueObjects;
using Pulse.Domain.Channels;

namespace Pulse.App.Handlers.Workflows.Commands.Versions;

public sealed record AddWorkflowVersionStepCommand : ICommand<ErrorOr<WorkflowVersionStepDto>>
{
    public required WorkflowId WorkflowId { get; init; }
    
    public required WorkflowVersionId WorkflowVersionId { get; init; }
    
    public required ChannelDto Channel { get; init; }
    
    public required Dictionary<string, string> Parameters { get; init; } = new();
}

public sealed class AddWorkflowVersionStepCommandAuthorizer : ApiKeyPermissionAuthorizer<AddWorkflowVersionStepCommand>;

public sealed class AddWorkflowVersionStepCommandHandler : 
    ICommandHandler<AddWorkflowVersionStepCommand, ErrorOr<WorkflowVersionStepDto>>
{
    private readonly IWorkflowRepository _workflowRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddWorkflowVersionStepCommandHandler(
        IWorkflowRepository workflowRepository,
        IUnitOfWork unitOfWork)
    {
        _workflowRepository = workflowRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<WorkflowVersionStepDto>> Handle(AddWorkflowVersionStepCommand command,
        CancellationToken cancellationToken)
    {
        // Fetch
        var specification = new WorkflowByIdSpecification(command.WorkflowId);
        var workflow = await _workflowRepository.SearchOne(specification, cancellationToken);

        if (workflow == null)
        {
            return Error.NotFound();
        }

        // The workflow version is strictly speaking not required, as a step can
        // only be added to the (single) version currently in draft.
        // However, having the client explicitly provide the version will avoid race conditions.
        var workflowVersion = workflow.Versions.Find(command.WorkflowVersionId);

        if (workflowVersion == null)
        {
            return Error.NotFound();
        }

        // Step definition
        var stepDefinitionResult = command.Channel switch
        {
            ChannelDto.Email => ProviderWorkflowStepDefinition.ForEmail(command.Parameters.AsParameterValues()),
            _ => throw new NotImplementedException(command.Channel.ToString())
        };

        if (stepDefinitionResult.IsError)
        {
            return stepDefinitionResult.Errors;
        }

        // Add (will trip when the version is no longer in draft)
        var step = workflowVersion.AddStep(stepDefinitionResult.Value);
        
        _workflowRepository.Update(workflow);
        await _unitOfWork.Commit(cancellationToken);

        return step.ToDto();
    }
}