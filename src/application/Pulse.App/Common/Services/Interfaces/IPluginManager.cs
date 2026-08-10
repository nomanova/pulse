using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Pulse.Plugin;

namespace Pulse.App.Common.Services.Interfaces;

public interface IPluginManager
{
    IReadOnlyCollection<PluginMetadata> Catalog { get; }

    Task<PluginInvocationResult> InvokeAsync(string pluginId, PluginInvocationRequest request,
        CancellationToken cancellationToken);
}