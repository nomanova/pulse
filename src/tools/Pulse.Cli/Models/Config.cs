using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Pulse.Cli.Models;

public sealed record Config
{
    public uint Version { get; init; } = 1;

    public Dictionary<string, Server> Servers { get; init; } = [];

    [JsonInclude]
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
    
    public void SetOrganization(string name)
    {
        Context = Context with
        {
            OrganizationName = name,
            ApplicationName = null,
            EnvironmentName = null
        };
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
    
    public void SignOut()
    {
        if (!HasServer())
        {
            throw new CliException("No server selected");
        }
        
        Servers[Context.ServerName!] = Servers[Context.ServerName!] with
        {
            AccessToken = null
        };
    }
}