using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Pulse.App.Common.Services.Interfaces;
using Pulse.App.Handlers.Connections.Common;
using Pulse.App.Handlers.Connections.Common.Specifications;
using Pulse.Domain.Aggregates.Connections;
using Pulse.Domain.Aggregates.Environments;
using Pulse.Domain.Channels;
using Pulse.Plugin;

namespace Pulse.App.Handlers.WorkflowInstances.Common;

public sealed record ProviderContext(Connection Connection, IPlugin Plugin);

public interface IProviderResolver
{
    Task<ProviderContext?> TryResolveFor(
        EnvironmentId environmentId, 
        Channel channel, 
        CancellationToken cancellationToken = default);
}

public sealed class ProviderResolver : IProviderResolver
{
    private readonly IPluginManager _pluginManager;
    private readonly IConnectionRepository _connectionRepository;

    public ProviderResolver(
        IPluginManager pluginManager,
        IConnectionRepository connectionRepository)
    {
        _pluginManager = pluginManager;
        _connectionRepository = connectionRepository;
    }

    public async Task<ProviderContext?> TryResolveFor(
        EnvironmentId environmentId, Channel channel, CancellationToken cancellationToken = default)
    {
        // Resolve connection
        var specification = new ConnectionByEnvironmentSpecification(environmentId, channel);
        var connections = await _connectionRepository.Search(specification, cancellationToken);

        if (connections.Count == 0)
        {
            return null;
        }

        // TODO - resolve the best connection based on policy
        var connection = connections[0];

        // Resolve plugin

        // TODO - take into account the plugin version
        var plugin = _pluginManager.TryGet(connection.PluginId);

        return plugin == null ? null : new ProviderContext(connection, plugin);
    }
}