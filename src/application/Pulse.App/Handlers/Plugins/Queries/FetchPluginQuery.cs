using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using Pulse.App.Common.Authorization.Policies;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Common.Services.Interfaces;
using Pulse.App.Dto.Plugins;
using Pulse.App.Handlers.Plugins.Common;

namespace Pulse.App.Handlers.Plugins.Queries;

public sealed record FetchPluginQuery : IQuery<ErrorOr<PluginDto>>
{
    public string? PluginId { get; init; }
}

public sealed class FetchPluginQueryAuthorizer : PermissionAuthorizer<FetchPluginQuery>;

public sealed class FetchPluginQueryHandler : IQueryHandler<FetchPluginQuery, ErrorOr<PluginDto>>
{
    private readonly IPluginManager _pluginManager;

    public FetchPluginQueryHandler(IPluginManager pluginManager)
    {
        _pluginManager = pluginManager;
    }

    public Task<ErrorOr<PluginDto>> Handle(FetchPluginQuery query, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(query.PluginId))
        {
            return Task.FromResult<ErrorOr<PluginDto>>(Error.NotFound());
        }

        var plugin = _pluginManager.TryGet(query.PluginId);

        return plugin == null
            ? Task.FromResult<ErrorOr<PluginDto>>(Error.NotFound())
            : Task.FromResult<ErrorOr<PluginDto>>(plugin.ToDto());
    }
}