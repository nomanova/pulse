using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Pulse.Plugin.Providers;

public interface IProviderPlugin : IPlugin<ProviderPluginInvocationRequest>
{
    ProviderChannel Channel { get; }
    
    List<PluginParameterDefinition> ConnectionParameters { get; }
    
    Task<PluginResult> CanConnect(
        List<PluginParameterValue> connectionParameters, CancellationToken cancellationToken = default);
}
