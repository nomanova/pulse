using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Pulse.Domain.Common.Models.Enums;
using Pulse.Plugin;
using Pulse.Plugin.Providers;

namespace Pulse.App.Common.Services.Interfaces;

public interface IPluginManager
{
    IReadOnlyCollection<PluginMetadata> GetCatalog(Channel? channel = null);
    
    Task<PluginResult> InvokeProvider(string pluginId, ProviderPluginInvocationRequest request,
        CancellationToken cancellationToken);
}