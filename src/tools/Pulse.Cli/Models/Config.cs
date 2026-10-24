using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Pulse.Cli.Models;

public sealed record Config
{
    public uint Version { get; init; } = 1;

    public Dictionary<string, Server> Servers { get; init; } = [];

    [JsonInclude]
    public Context Context { get; private set; } = new();

    public void SetServer(string key)
    {
        Context = new Context
        {
            ServerKey = key
        };
    }

    public void ClearServer()
    {
        Context = new Context();
    }
    
    public bool HasServer()
    {
        return Context.ServerKey is not null &&
               Servers.ContainsKey(Context.ServerKey);
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
        return Context.ServerKey is null ? null : Servers[Context.ServerKey];
    }
    
    public void SetOrganization(string id, string key)
    {
        Context = Context with
        {
            Organization = new KeyedIdentity(id, key),
            Application = null,
            Environment = null
        };
    }

    public void SetApplication(string id, string key)
    {
        Context = Context with
        {
            Application = new KeyedIdentity(id, key),
            Environment = null
        };
    }

    public void SetEnvironment(string id, string key)
    {
        Context = Context with
        {
            Environment = new KeyedIdentity(id, key)
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

        Servers[Context.ServerKey!] = Servers[Context.ServerKey!] with
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
        
        Servers[Context.ServerKey!] = Servers[Context.ServerKey!] with
        {
            AccessToken = null
        };
    }
}