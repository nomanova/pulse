using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Pulse.Domain.Channels;

namespace Pulse.Plugin;

public interface IPlugin
{
    PluginMetadata Metadata { get; }
    
    Task Initialize(IPluginHostContext hostContext, CancellationToken cancellationToken = default);
    
    List<ParameterDefinition> InvocationParameters { get; }
    
    ParameterValidationResult CanInvoke(List<ParameterValue> invocationParameters);
    
    Task Invoke(PluginInvocationRequest request, CancellationToken cancellationToken = default);
}

public interface IPlugin<in T> : IPlugin where T : PluginInvocationRequest
{
    Task Invoke(T request, CancellationToken cancellationToken = default);

    async Task IPlugin.Invoke(
        PluginInvocationRequest request,
        CancellationToken cancellationToken)
    {
        if (request is not T typedRequest)
        {
            throw new PluginException(
                $"Invalid request type. Expected {typeof(T).Name}, got {request.GetType().Name}.");
        }

        await Invoke(typedRequest, cancellationToken);
    }
}