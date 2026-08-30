using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using Pulse.Domain.Channels;
using Pulse.Domain.Common.Models.Enums;
using Pulse.Plugin;

namespace Pulse.App.Common.Services.Interfaces;

public interface IPluginManager
{
    IReadOnlyCollection<IPlugin> GetCatalog(Channel? channel = null);

    IPlugin? TryGet(string pluginId);
    
    Task<ErrorOr<Success>> Invoke(string pluginId, PluginInvocationRequest request,
        CancellationToken cancellationToken);
}