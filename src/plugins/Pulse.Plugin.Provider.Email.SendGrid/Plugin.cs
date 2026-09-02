using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Pulse.Domain.Channels;
using Pulse.Domain.Channels.Definitions;
using Pulse.Plugin.Providers;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Pulse.Plugin.Provider.Email.SendGrid;

public sealed class Plugin : EmailProviderPlugin
{
    private const string ApiKeyConnectionParameter = "apiKey";

    private static readonly ParameterValidationError ErrApiKeyMissing =
        new(ApiKeyConnectionParameter, "API Key is required");

    private IPluginHostContext? _context;

    public override PluginMetadata Metadata => new(
        Id: "com.nomanova.pulse.plugin.provider.email.sendgrid",
        DisplayName: "SendGrid Email",
        Version: "1.0.0",
        Description: "Send emails using SendGrid"
    );
    
    public override Task Initialize(IPluginHostContext hostContext, CancellationToken cancellationToken = default)
    {
        _context = hostContext;
        return Task.CompletedTask;
    }

    public override List<ParameterDefinition> ConnectionParameters =>
    [
        new(ApiKeyConnectionParameter, "API Key", "SendGrid API Key")
    ];

    public override Task<ParameterValidationResult> CanConnect(
        List<ParameterValue> connectionParameters, CancellationToken cancellationToken = default)
    {
        var errors = new List<ParameterValidationError>();

        // Api key
        var apiKey = connectionParameters.GetValue(ApiKeyConnectionParameter);

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            errors.Add(ErrApiKeyMissing);
        }

        // TODO - add API key validation

        return Task.FromResult(ParameterValidationResult.For(errors));
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

        var apiKey = connectionParameters.GetValue(ApiKeyConnectionParameter);

        var fromEmail = invocationParameters.GetValue(EmailProviderDefinition.FromEmailParameterKey);
        var fromName = invocationParameters.GetValue(EmailProviderDefinition.FromNameParameterKey);

        var toEmail = invocationParameters.GetValue(EmailProviderDefinition.ToEmailParameterKey);
        var toName = invocationParameters.GetValue(EmailProviderDefinition.ToNameParameterKey);

        var client = new SendGridClient(apiKey);
        var fromAddress = new EmailAddress(fromEmail, fromName);
        var toAddress = new EmailAddress(toEmail, toName);

        var subject = invocationParameters.GetValue(EmailProviderDefinition.SubjectParameterKey);
        var body = invocationParameters.GetValue(EmailProviderDefinition.BodyParameterKey);

        var message = MailHelper.CreateSingleEmail(
            fromAddress, toAddress, subject, null, body);

        _context.LogInformation($"SendGrid email to {toAddress.Email} ({subject})");

        Response? response;

        try
        {
            response = await client
                .SendEmailAsync(message, cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _context.LogError($"SendGrid request failed: {ex.Message}");
            throw new PluginException(ex.Message, ex);
        }

        if (!response.IsSuccessStatusCode)
        {
            var responseBody = await response.Body.ReadAsStringAsync(cancellationToken)
                .ConfigureAwait(false);

            throw new PluginException(
                $"SendGrid request failed with status {(int)response.StatusCode}: {responseBody}");
        }
    }
}