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

    public Context Context { get; private set; } = new();

    public void SetServer(string name)
    {
        Context = new Context
        {
            ServerName = name
        };
    }

    public void ClearServer()
    {
        Context = new Context();
    }

    public bool HasServer()
    {
        return Context.ServerName is not null &&
               Servers.ContainsKey(Context.ServerName);
    }

    public Server? CurrentServer()
    {
        return Context.ServerName is null ? null : Servers[Context.ServerName];
    }
    
    public void SignIn(string accessToken)
    {
        if (!HasServer())
        {
            throw new CliException("No server selected");
        }

        Servers[Context.ServerName!] = Servers[Context.ServerName!] with
        {
            AccessToken = accessToken
        };
    }
}