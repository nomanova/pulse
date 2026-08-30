using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Pulse.Domain.Channels;

namespace Pulse.Plugin.Providers;

public abstract class EmailProviderPlugin : IProviderPlugin
{
    public abstract PluginMetadata Metadata { get; }

    public abstract Task Initialize(IPluginHostContext hostContext, CancellationToken cancellationToken = default);

    public List<ParameterDefinition> InvocationParameters =>
        EmailProviderDefinition.Instance.InvocationParameters;

    public ParameterValidationResult CanInvoke(List<ParameterValue> invocationParameters)
    {
        return EmailProviderDefinition.Instance.CanInvoke(invocationParameters);
    }

    public abstract Task Invoke(
        ProviderPluginInvocationRequest request, CancellationToken cancellationToken = default);

    public Channel Channel => EmailProviderDefinition.Instance.Channel;

    public abstract List<ParameterDefinition> ConnectionParameters { get; }

    public abstract Task<ParameterValidationResult> CanConnect(List<ParameterValue> connectionParameters,
        CancellationToken cancellationToken = default);
}