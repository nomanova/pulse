using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using Pulse.App.Common.Authorization.Policies;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Common.Services.Interfaces;
using Pulse.Domain.Channels;

namespace Pulse.App.Handlers.Plugins.Commands;

public sealed record VerifyInvokePluginCommand : ICommand<ErrorOr<Success>>
{
    public string? PluginId { get; init; }

    public required Dictionary<string, string> Parameters { get; init; } = new();
}

public sealed class VerifyInvokePluginCommandAuthorizer : PermissionAuthorizer<VerifyInvokePluginCommand>;

public sealed class VerifyInvokePluginCommandHandler : ICommandHandler<VerifyInvokePluginCommand, ErrorOr<Success>>
{
    private readonly IPluginManager _pluginManager;

    public VerifyInvokePluginCommandHandler(IPluginManager pluginManager)
    {
        _pluginManager = pluginManager;
    }

    public async Task<ErrorOr<Success>> Handle(VerifyInvokePluginCommand command, CancellationToken cancellationToken)
    {
        // Fetch plugin
        if (string.IsNullOrEmpty(command.PluginId))
        {
            return Error.NotFound();
        }

        var plugin = _pluginManager.TryGet(command.PluginId);

        if (plugin is null)
        {
            return Error.NotFound();
        }

        // Verify invocation
        var parameters = command.Parameters.AsParameterValues();
        var invocationResult = plugin.CanInvoke(parameters);

        if (!invocationResult.IsSuccess)
        {
            var errors = invocationResult.Errors.ToList().ConvertAll(error =>
                Error.Validation(error.ParameterKey, error.Message));
            return (dynamic)errors;
        }

        return Result.Success;
    }
}