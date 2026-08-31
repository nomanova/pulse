using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using Pulse.App.Common.Authorization.Policies;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Common.Services.Interfaces;
using Pulse.Domain.Channels;
using Pulse.Domain.Common.Errors;
using Pulse.Plugin.Providers;

namespace Pulse.App.Handlers.Plugins.Commands;

public sealed record VerifyConnectPluginCommand : ICommand<ErrorOr<Success>>
{
    public string? PluginId { get; init; }

    public required Dictionary<string, string> Parameters { get; init; } = new();
}

public sealed class VerifyConnectPluginCommandAuthorizer : PermissionAuthorizer<VerifyConnectPluginCommand>;

public sealed class VerifyConnectPluginCommandHandler :
    ICommandHandler<VerifyConnectPluginCommand, ErrorOr<Success>>
{
    private readonly IPluginManager _pluginManager;

    public VerifyConnectPluginCommandHandler(IPluginManager pluginManager)
    {
        _pluginManager = pluginManager;
    }

    public async Task<ErrorOr<Success>> Handle(VerifyConnectPluginCommand command, CancellationToken cancellationToken)
    {
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

        return Result.Success;
    }
}