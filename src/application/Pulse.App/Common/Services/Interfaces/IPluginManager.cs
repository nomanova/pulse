using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Pulse.Domain.Common.Models.Enums;
using Pulse.Plugin;

namespace Pulse.App.Common.Services.Interfaces;

public interface IPluginManager
{
    IReadOnlyCollection<PluginMetadata> GetCatalog(Channel? channel = null);

    IPlugin? TryGet(string pluginId);
    
    Task<PluginResult> Invoke(string pluginId, PluginInvocationRequest request,
        CancellationToken cancellationToken);
}