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

    public bool HasOrganization()
    {
        return HasServer() && Context.Organization is not null;
    }

    public bool HasApplication()
    {
        return HasOrganization() && Context.Application is not null;
    }
    
    public Server? CurrentServer()
    {
        return Context.ServerName is null ? null : Servers[Context.ServerName];
    }
    
    public void SetOrganization(string id, string name)
    {
        Context = Context with
        {
            Organization = new NamedIdentity(id, name),
            Application = null,
            Environment = null
        };
    }

    public void SetApplication(string id, string name)
    {
        Context = Context with
        {
            Application = new NamedIdentity(id, name),
            Environment = null
        };
    }

    public void SetEnvironment(string id, string name)
    {
        Context = Context with
        {
            Environment = new NamedIdentity(id, name)
        };
    }
    
    public void ClearOrganization()
    {
        Context = Context with
        {
            Organization = null,
            Application = null,
            Environment = null
        };
    }
    
    public void ClearApplication()
    {
        Context = Context with
        {
            Application = null,
            Environment = null
        };
    }
    
    public void ClearEnvironment()
    {
        Context = Context with
        {
            Environment = null
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