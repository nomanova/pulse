using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Pulse.Domain.Channels;
using Pulse.Domain.Channels.Definitions;
using Pulse.Plugin.Providers;

namespace Pulse.Plugin.Provider.Sms.Console;

public sealed class Plugin : SmsProviderPlugin
{
    private const string Separator = "*************************************************************";

    private IPluginHostContext? _context;

    public override PluginMetadata Metadata => new(
        Id: "com.nomanova.pulse.plugin.provider.sms.console",
        DisplayName: "Console SMS",
        Version: "1.0.0",
        Description: "Print SMS messages on the log console (for debugging purposes)"
    );

    public override Task Initialize(IPluginHostContext hostContext, CancellationToken cancellationToken = default)
    {
        _context = hostContext;
        return Task.CompletedTask;
    }

    public override List<ParameterDefinition> ConnectionParameters => [];

    public override Task<ParameterValidationResult> CanConnect(List<ParameterValue> connectionParameters,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(ParameterValidationResult.Success());
    }

    public override async Task Invoke(ProviderPluginInvocationRequest request,
        CancellationToken cancellationToken = default)
    {
        if (_context == null)
        {
            throw new PluginException("Plugin not initialized");
        }

        var connectionParameters = request.ConnectionParameters;
        var connectResult = await CanConnect(connectionParameters, cancellationToken);

        if (!connectResult.IsSuccess)
        {
            throw new PluginException($"Connection validation failed, call {nameof(CanConnect)} first");
        }

        var invocationParameters = request.InvocationParameters;
        var invokeResult = CanInvoke(invocationParameters);

        if (!invokeResult.IsSuccess)
        {
            throw new PluginException($"Invocation validation failed, call {nameof(CanInvoke)} first");
        }

        var toPhoneNumber = invocationParameters.GetValue(SmsProviderDefinition.ToPhoneNumberParameterKey);
        var body = invocationParameters.GetValue(SmsProviderDefinition.BodyParameterKey);

        var builder = new StringBuilder();

        builder.AppendLine();
        builder.AppendLine(Separator);
        builder.AppendLine($"To:      {toPhoneNumber}");

        builder.AppendLine(Separator);
        builder.AppendLine(body);
        builder.AppendLine(Separator);

        _context.LogInformation(builder.ToString());
    }
}