using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Pulse.Domain.Channels;

namespace Pulse.Plugin.Providers;

public interface IProviderPlugin : IPlugin<ProviderPluginInvocationRequest>
{
    Channel Channel { get; }
    
    List<ParameterDefinition> ConnectionParameters { get; }
    
    Task<ParameterValidationResult> CanConnect(
        List<ParameterValue> connectionParameters, CancellationToken cancellationToken = default);
}
