using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Pulse.Domain.Channels;
using Pulse.Plugin.Providers;

namespace Pulse.Plugin.Provider.Email.Console;

public sealed class Plugin : EmailProviderPlugin
{
    private const string Separator = "*************************************************************";

    private IPluginHostContext _context = null!;

    public override PluginMetadata Metadata => new(
        Id: "com.nomanova.pulse.plugin.provider.email.console",
        DisplayName: "Console Email",
        Version: "1.0.0",
        Description: "Print emails on the log console (for debugging purposes)"
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

        var fromEmail = connectionParameters.GetValue(EmailProviderDefinition.FromEmailParameterKey);
        var fromName = connectionParameters.GetValue(EmailProviderDefinition.FromNameParameterKey);

        var toEmail = invocationParameters.GetValue(EmailProviderDefinition.ToEmailParameterKey);
        var toName = invocationParameters.GetValue(EmailProviderDefinition.ToNameParameterKey);

        var subject = invocationParameters.GetValue(EmailProviderDefinition.SubjectParameterKey);
        var body = invocationParameters.GetValue(EmailProviderDefinition.BodyParameterKey);

        var builder = new StringBuilder();

        builder.AppendLine();
        builder.AppendLine(Separator);
        builder.AppendLine($"From:    {fromName} - {fromEmail}");
        builder.AppendLine($"To:      {toName} - {toEmail}");
        builder.AppendLine($"Subject: {subject}");

        builder.AppendLine(Separator);
        builder.AppendLine(body);

        builder.AppendLine(Separator);
        builder.AppendLine();

        _context.LogInformation(builder.ToString());
    }
}