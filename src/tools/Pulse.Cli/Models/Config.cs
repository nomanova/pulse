using System.Collections.Generic;

namespace Pulse.Cli.Models;

public sealed record Server
{
    public required string Url { get; init; }

    public string? AccessToken { get; init; }
}

public sealed record Context
{
    public string? ServerName { get; init; }

    public string? OrganizationName { get; init; }

    public string? ApplicationName { get; init; }

    public string? EnvironmentName { get; init; }
}

public sealed record Config
{
    public uint Version { get; init; } = 1;

    public Dictionary<string, Server> Servers { get; init; } = [];

    public Context Context { get; init; } = new();
}