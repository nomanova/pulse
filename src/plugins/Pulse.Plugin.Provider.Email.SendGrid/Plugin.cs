using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Pulse.Plugin.Providers;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Pulse.Plugin.Provider.Email.SendGrid;

public sealed class Plugin : EmailProviderPlugin
{
    private const string ApiKeyConnectionParameter = "api-key";

    private static readonly PluginError ErrApiKeyMissing =
        new("API Key is required", ApiKeyConnectionParameter);

    public override PluginMetadata Metadata => new(
        Id: "com.nomanova.pulse.plugin.provider.email.sendgrid",
        DisplayName: "SendGrid Email",
        Version: "1.0.0",
        Description: "Send emails using SendGrid"
    );

    public override List<PluginParameterDefinition> ConnectionParameters =>
    [
        ..base.ConnectionParameters,
        new(ApiKeyConnectionParameter, "API Key", "SendGrid API Key")
    ];

    public override async Task<PluginResult> CanConnect(
        List<PluginParameterValue> connectionParameters, CancellationToken cancellationToken)
    {
        var errors = new List<PluginError>();

        var baseResult = await base.CanConnect(connectionParameters, cancellationToken)
            .ConfigureAwait(false);

        if (!baseResult.IsSuccess)
        {
            errors.AddRange(baseResult.Errors);
        }

        // Api key
        var apiKey = connectionParameters.GetValue(ApiKeyConnectionParameter);

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            errors.Add(ErrApiKeyMissing);
        }

        return errors.Count > 0
            ? PluginResult.Failure(errors)
            : PluginResult.Success();
    }

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

        var apiKey = connectionParameters.GetValue(ApiKeyConnectionParameter);
        var fromEmail = connectionParameters.GetValue(FromEmailConnectionParameter);
        var fromName = connectionParameters.GetValue(FromNameConnectionParameter);

        var toEmail = invocationParameters.GetValue(ToEmailInvocationParameter);
        var toName = invocationParameters.GetValue(ToNameInvocationParameter);

        var client = new SendGridClient(apiKey);
        var fromAddress = new EmailAddress(fromEmail, fromName);
        var toAddress = new EmailAddress(toEmail, toName);

        var subject = invocationParameters.GetValue(SubjectInvocationParameter);
        var body = invocationParameters.GetValue(BodyInvocationParameter);

        var message = MailHelper.CreateSingleEmail(
            fromAddress, toAddress, subject, null, body);

        Context.LogInformation($"SendGrid email to {toAddress.Email} ({subject})");

        try
        {
            var response = await client
                .SendEmailAsync(message, cancellationToken)
                .ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                var responseBody = await response.Body.ReadAsStringAsync(cancellationToken)
                    .ConfigureAwait(false);

                return PluginResult.Failure(new PluginError(
                    $"SendGrid request failed with status {(int)response.StatusCode}: {responseBody}"));
            }
        }
        catch (Exception ex)
        {
            Context.LogError($"SendGrid request failed: {ex.Message}");
            return PluginResult.Failure(new PluginError(ex.Message));
        }

        return PluginResult.Success();
    }
}