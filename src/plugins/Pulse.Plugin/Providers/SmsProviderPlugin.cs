using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Pulse.Domain.Channels;
using Pulse.Domain.Channels.Definitions;

namespace Pulse.Plugin.Providers;

public abstract class SmsProviderPlugin : IProviderPlugin
{
    public abstract PluginMetadata Metadata { get; }

    public abstract Task Initialize(IPluginHostContext hostContext, CancellationToken cancellationToken = default);

    public List<ParameterDefinition> InvocationParameters =>
        SmsProviderDefinition.Instance.InvocationParameters;

    public ParameterValidationResult CanInvoke(List<ParameterValue> invocationParameters)
    {
        return SmsProviderDefinition.Instance.CanInvoke(invocationParameters);
    }

    public abstract Task Invoke(
        ProviderPluginInvocationRequest request, CancellationToken cancellationToken = default);

    public Channel Channel => SmsProviderDefinition.Instance.Channel;

    public abstract List<ParameterDefinition> ConnectionParameters { get; }

    public abstract Task<ParameterValidationResult> CanConnect(List<ParameterValue> connectionParameters,
        CancellationToken cancellationToken = default);
}