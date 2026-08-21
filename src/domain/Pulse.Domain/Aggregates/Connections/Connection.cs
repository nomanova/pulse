using System.Collections.Generic;
using Pulse.Domain.Aggregates.Environments;
using Pulse.Domain.Common.Models.Entities;
using Pulse.Domain.Common.Models.Enums;
using Pulse.Domain.Common.Services;

namespace Pulse.Domain.Aggregates.Connections;

public sealed record ConnectionId : EntityId<ConnectionId, Connection>;

public sealed class Connection : DomainEntity<ConnectionId>, IEnvironmentScoped
{
    public EnvironmentId EnvironmentId { get; private set; } = null!;

    public Channel Channel { get; private set; }

    public string PluginId { get; private set; } = null!;

    public Dictionary<string, string> Parameters { get; private set; } = new();

    private Connection()
    {
    }

    private Connection(
        ConnectionId id,
        EnvironmentId environmentId,
        Channel channel,
        string pluginId,
        Dictionary<string, string> parameters) : base(id)
    {
        EnvironmentId = environmentId;
        Channel = channel;
        PluginId = pluginId;
        Parameters = parameters;
    }

    public static Connection Create(
        Environment environment,
        Channel channel,
        string pluginId,
        Dictionary<string, string> parameters)
    {
        var id = IdentityProvider.New<ConnectionId>();

        var connection = new Connection(id, environment.Id, channel, pluginId, parameters);
        connection.SetCreated();

        return connection;
    }
}