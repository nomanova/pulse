using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using Pulse.App.Common.Authorization.Policies;
using Pulse.App.Common.Database;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Common.Errors;
using Pulse.App.Common.Security.Interfaces;
using Pulse.App.Dto.Common;
using Pulse.App.Dto.WorkflowInstances;
using Pulse.App.Handlers.Connections.Common;
using Pulse.App.Handlers.Connections.Common.Specifications;
using Pulse.App.Handlers.WorkflowInstances.Common;
using Pulse.Domain.Aggregates.WorkflowInstances;
using Pulse.Domain.Aggregates.Workflows.ValueObjects;
using Pulse.Domain.Channels;

namespace Pulse.App.Handlers.Workflows.Commands;

public sealed record AdHocWorkflowCommand : ICommand<ErrorOr<WorkflowInstanceDto>>
{
    public ChannelDto Channel { get; init; }

    public required Dictionary<string, string> Parameters { get; init; } = new();
}

public sealed class AdHocWorkflowCommandAuthorizer : ApiKeyAuthorizer<AdHocWorkflowCommand>;

public sealed class AdHocWorkflowCommandHandler :
    ICommandHandler<AdHocWorkflowCommand, ErrorOr<WorkflowInstanceDto>>
{
    private readonly IEnvironmentProvider _environmentProvider;
    private readonly IConnectionRepository _connectionRepository;
    private readonly IWorkflowInstanceRepository _workflowInstanceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AdHocWorkflowCommandHandler(
        IEnvironmentProvider environmentProvider,
        IConnectionRepository connectionRepository,
        IWorkflowInstanceRepository workflowInstanceRepository,
        IUnitOfWork unitOfWork)
    {
        _environmentProvider = environmentProvider;
        _connectionRepository = connectionRepository;
        _workflowInstanceRepository = workflowInstanceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<WorkflowInstanceDto>> Handle(AdHocWorkflowCommand command,
        CancellationToken cancellationToken)
    {
        var environment = await _environmentProvider.Get(cancellationToken);
        var channel = (Channel)command.Channel;

        // Check the presence of at least one provider
        var specification = new ConnectionByEnvironmentSpecification(environment.Id, channel);
        var connection = await _connectionRepository.SearchOne(specification, cancellationToken);

        if (connection == null)
        {
            return ApplicationErrors.Workflow.ProviderMissing;
        }

        // Create workflow instance
        var stepDefinitionResult = channel switch
        {
            Channel.Email => ProviderWorkflowStepDefinition.ForEmail(command.Parameters.AsParameterValues()),
            _ => throw new NotImplementedException(command.Channel.ToString())
        };

        if (stepDefinitionResult.IsError)
        {
            return stepDefinitionResult.Errors;
        }

        var workflowInstance = WorkflowInstance.CreateAdHoc(environment, stepDefinitionResult.Value);

        _workflowInstanceRepository.Add(workflowInstance);
        await _unitOfWork.Commit(cancellationToken);

        return workflowInstance.ToDto();
    }
}