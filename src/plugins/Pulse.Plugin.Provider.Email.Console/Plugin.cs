using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Pulse.Plugin.Providers;

namespace Pulse.Plugin.Provider.Email.Console;

public sealed class Plugin : EmailProviderPlugin
{
    private const string Separator = "*************************************************************";

    public override PluginMetadata Metadata => new(
        Id: "com.nomanova.pulse.plugin.provider.email.console",
        DisplayName: "Console Email",
        Version: "1.0.0",
        Description: "Print emails on the log console (for debugging purposes)"
    );

    public override async Task<PluginResult> Invoke(ProviderPluginInvocationRequest request,
        CancellationToken cancellationToken)
    {
        var connectionParameters = request.ConnectionParameters;
        var connectResult = await CanConnect(connectionParameters, cancellationToken);

        if (!connectResult.IsSuccess)
        {
            return connectResult;
        }

        var invocationParameters = request.InvocationParameters;
        var invokeResult = await CanInvoke(invocationParameters, cancellationToken);

        if (!invokeResult.IsSuccess)
        {
            return invokeResult;
        }

        var fromEmail = connectionParameters.GetValue(FromEmailConnectionParameter);
        var fromName = connectionParameters.GetValue(FromNameConnectionParameter);

        var toEmail = invocationParameters.GetValue(ToEmailInvocationParameter);
        var toName = invocationParameters.GetValue(ToNameInvocationParameter);

        var subject = invocationParameters.GetValue(SubjectInvocationParameter);
        var body = invocationParameters.GetValue(BodyInvocationParameter);

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

        Context.LogInformation(builder.ToString());

        return PluginResult.Success();
    }
}