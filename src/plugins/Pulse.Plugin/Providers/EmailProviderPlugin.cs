using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Pulse.Plugin.Providers;

public interface IEmailProviderPlugin : IProviderPlugin;

public abstract class EmailProviderPlugin : IEmailProviderPlugin
{
    protected const string FromEmailConnectionParameter = "from-email";
    protected const string FromNameConnectionParameter = "from-name";

    protected const string ToEmailInvocationParameter = "to-email";
    protected const string ToNameInvocationParameter = "to-name";
    protected const string SubjectInvocationParameter = "subject";
    protected const string BodyInvocationParameter = "body";

    private static readonly PluginError ErrFromEmailMissing =
        new("From email is required", FromEmailConnectionParameter);

    private static readonly PluginError ErrToEmailMissing =
        new("To email is required", ToEmailInvocationParameter);

    private static readonly PluginError ErrSubjectMissing =
        new("Subject is required", SubjectInvocationParameter);

    private static readonly PluginError ErrBodyMissing =
        new("Body is required", BodyInvocationParameter);

    protected IPluginHostContext Context = null!;

    public abstract PluginMetadata Metadata { get; }

    public ProviderChannel Channel => ProviderChannel.Email;
    
    public virtual Task Initialize(IPluginHostContext hostContext, CancellationToken cancellationToken)
    {
        Context = hostContext;
        return Task.CompletedTask;
    }

    public virtual List<PluginParameterDefinition> ConnectionParameters =>
    [
        new(FromEmailConnectionParameter, "From email", "From email"),
        new(FromNameConnectionParameter, "From name", "Form name", false)
    ];

    public List<PluginParameterDefinition> InvocationParameters =>
    [
        new(ToEmailInvocationParameter, "To email", "To email"),
        new(ToNameInvocationParameter, "To name", "To name", false),
        new(SubjectInvocationParameter, "Subject", "Email subject"),
        new(BodyInvocationParameter, "Body", "Email body")
    ];

    public virtual Task<PluginResult> CanConnect(List<PluginParameterValue> connectionParameters,
        CancellationToken cancellationToken)
    {
        // From email
        var fromEmail = connectionParameters.GetValue(FromEmailConnectionParameter);

        if (string.IsNullOrWhiteSpace(fromEmail))
        {
            return Task.FromResult(PluginResult.Failure(ErrFromEmailMissing));
        }

        return Task.FromResult(PluginResult.Success());
    }

    public Task<PluginResult> CanInvoke(List<PluginParameterValue> invocationParameters,
        CancellationToken cancellationToken)
    {
        // To email
        var toEmail = invocationParameters.GetValue(ToEmailInvocationParameter);

        if (string.IsNullOrWhiteSpace(toEmail))
        {
            return Task.FromResult(PluginResult.Failure(ErrToEmailMissing));
        }

        // Subject
        var subject = invocationParameters.GetValue(SubjectInvocationParameter);

        if (string.IsNullOrWhiteSpace(subject))
        {
            return Task.FromResult(PluginResult.Failure(ErrSubjectMissing));
        }

        // Body
        var body = invocationParameters.GetValue(BodyInvocationParameter);

        if (string.IsNullOrWhiteSpace(body))
        {
            return Task.FromResult(PluginResult.Failure(ErrBodyMissing));
        }

        return Task.FromResult(PluginResult.Success());
    }

    public abstract Task<PluginResult> Invoke(ProviderPluginInvocationRequest request,
        CancellationToken cancellationToken);
}