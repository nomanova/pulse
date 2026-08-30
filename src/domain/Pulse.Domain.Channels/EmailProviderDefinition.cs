using System.Collections.Generic;

namespace Pulse.Domain.Channels;

public sealed class EmailProviderDefinition : IProviderDefinition
{
    public static readonly EmailProviderDefinition Instance = new();

    public const string FromEmailParameterKey = "fromEmail";
    public const string FromNameParameterKey = "fromName";

    public const string ToEmailParameterKey = "toEmail";
    public const string ToNameParameterKey = "toName";
    public const string SubjectParameterKey = "subject";
    public const string BodyParameterKey = "body";

    private static readonly ParameterValidationError ErrFromEmailMissing =
        new(FromEmailParameterKey, "From email is required");

    private static readonly ParameterValidationError ErrToEmailMissing =
        new(ToEmailParameterKey, "To email is required");

    private static readonly ParameterValidationError ErrSubjectMissing =
        new(SubjectParameterKey, "Subject is required");

    private static readonly ParameterValidationError ErrBodyMissing =
        new(BodyParameterKey, "Body is required");

    public Channel Channel => Channel.Email;

    public List<ParameterDefinition> InvocationParameters =>
    [
        new(FromEmailParameterKey, "From email", "From email"),
        new(FromNameParameterKey, "From name", "Form name", false),
        new(ToEmailParameterKey, "To email", "To email"),
        new(ToNameParameterKey, "To name", "To name", false),
        new(SubjectParameterKey, "Subject", "Email subject"),
        new(BodyParameterKey, "Body", "Email body")
    ];

    public ParameterValidationResult CanInvoke(List<ParameterValue> invocationValues)
    {
        var errors = new List<ParameterValidationError>();

        // From email
        var fromEmail = invocationValues.GetValue(FromEmailParameterKey);

        if (string.IsNullOrWhiteSpace(fromEmail))
        {
            errors.Add(ErrFromEmailMissing);
        }

        // To email
        var toEmail = invocationValues.GetValue(ToEmailParameterKey);

        if (string.IsNullOrWhiteSpace(toEmail))
        {
            errors.Add(ErrToEmailMissing);
        }

        // Subject
        var subject = invocationValues.GetValue(SubjectParameterKey);

        if (string.IsNullOrWhiteSpace(subject))
        {
            errors.Add(ErrSubjectMissing);
        }

        // Body
        var body = invocationValues.GetValue(BodyParameterKey);

        if (string.IsNullOrWhiteSpace(body))
        {
            errors.Add(ErrBodyMissing);
        }

        return ParameterValidationResult.For(errors);
    }
}