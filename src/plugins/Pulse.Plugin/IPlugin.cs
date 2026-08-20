using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Pulse.Plugin;

public interface IPlugin
{
    PluginMetadata Metadata { get; }
    
    Task Initialize(IPluginHostContext hostContext, CancellationToken cancellationToken);
    
    List<PluginParameterDefinition> InvocationParameters { get; }
    
    Task<PluginResult> CanInvoke(
        List<PluginParameterValue> invocationParameters, CancellationToken cancellationToken);
    
    Task<PluginResult> Invoke(PluginInvocationRequest request, CancellationToken cancellationToken);
}

public interface IPlugin<in T> : IPlugin where T : PluginInvocationRequest
{
    Task<PluginResult> Invoke(T request, CancellationToken cancellationToken);

    async Task<PluginResult> IPlugin.Invoke(
        PluginInvocationRequest request,
        CancellationToken cancellationToken)
    {
        if (request is not T typedRequest)
        {
            return PluginResult.Failure(new PluginError(
                $"Invalid request type. Expected {typeof(T).Name}, got {request.GetType().Name}."));
        }

        return await Invoke(typedRequest, cancellationToken);
    }
}