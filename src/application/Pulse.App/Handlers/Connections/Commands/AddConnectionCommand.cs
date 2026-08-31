using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using Pulse.App.Common.Authorization.Policies;
using Pulse.App.Common.Database;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Common.Mappers;
using Pulse.App.Common.Services.Interfaces;
using Pulse.App.Dto.Common;
using Pulse.App.Handlers.Connections.Common;
using Pulse.App.Handlers.Environments.Common;
using Pulse.App.Handlers.Environments.Common.Specifications;
using Pulse.Domain.Aggregates.Connections;
using Pulse.Domain.Aggregates.Environments;
using Pulse.Domain.Channels;
using Pulse.Domain.Common.Errors;
using Pulse.Plugin.Providers;

namespace Pulse.App.Handlers.Connections.Commands;

public sealed record AddConnectionCommand : ICommand<ErrorOr<IdentityDto>>
{
    public required EnvironmentId EnvironmentId { get; init; }

    public required string? PluginId { get; init; }

    public required Dictionary<string, string> Parameters { get; init; } = new();
}

public sealed class AddConnectionCommandAuthorizer : PermissionAuthorizer<AddConnectionCommand>;

public class AddConnectionCommandHandler : ICommandHandler<AddConnectionCommand, ErrorOr<IdentityDto>>
{
    private readonly IPluginManager _pluginManager;
    private readonly IEnvironmentRepository _environmentRepository;
    private readonly IConnectionRepository _connectionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddConnectionCommandHandler(
        IPluginManager pluginManager,
        IEnvironmentRepository environmentRepository,
        IConnectionRepository connectionRepository,
        IUnitOfWork unitOfWork)
    {
        _pluginManager = pluginManager;
        _environmentRepository = environmentRepository;
        _connectionRepository = connectionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<IdentityDto>> Handle(AddConnectionCommand command, CancellationToken cancellationToken)
    {
        // Fetch environment
        var environmentSpecification = new EnvironmentByIdSpecification(command.EnvironmentId);
        var environment = await _environmentRepository.SearchOne(environmentSpecification, cancellationToken);

        if (environment == null)
        {
            return Error.NotFound();
        }

        // Fetch plugin
        if (string.IsNullOrEmpty(command.PluginId))
        {
            return Error.NotFound();
        }

        var plugin = _pluginManager.TryGet(command.PluginId);

        if (plugin is not IProviderPlugin providerPlugin)
        {
            return Error.NotFound();
        }

        // Verify connection
        var parameters = command.Parameters.AsParameterValues();
        var connectResult = await providerPlugin.CanConnect(parameters, cancellationToken);

        if (!connectResult.IsSuccess)
        {
            var errors = connectResult.Errors.Map();
            return (dynamic)errors;
        }

        // Create connection
        var connection = Connection.Create(
            environment, providerPlugin.Channel, providerPlugin.Metadata.Id, parameters);

        _connectionRepository.Add(connection);
        await _unitOfWork.Commit(cancellationToken);

        return connection.ToIdentityDto();
    }
}