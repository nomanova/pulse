using Pulse.App.Dto.Environments;

namespace Pulse.Web.Core.Models;

public record ObjectIdentity(string Id, string Name, string Key);

public static class ObjectIdentityExtensions
{
    public static ObjectIdentity ToIdentity(this EnvironmentDto environment) => 
        new(environment.Id, environment.Name, environment.Key);
}