using System.Collections.Generic;

namespace Pulse.Domain.Channels.Definitions;

public sealed class SmsProviderDefinition : IProviderDefinition
{
    public const string ToPhoneNumberParameterKey = "toPhoneNumber";
    public const string BodyParameterKey = "body";

    private const int MaxBodyLength = 160;

    private static readonly ParameterValidationError ErrToPhoneNumberMissing =
        new(ToPhoneNumberParameterKey, "To phone number is required");

    private static readonly ParameterValidationError ErrBodyMissing =
        new(BodyParameterKey, "Body is required");

    private static readonly ParameterValidationError ErrBodyMaxLength =
        new(BodyParameterKey, $"Body length exceeds {MaxBodyLength} characters");

    public static readonly SmsProviderDefinition Instance = new();

    public Channel Channel => Channel.Sms;

    public List<ParameterDefinition> InvocationParameters =>
    [
        new(ToPhoneNumberParameterKey, "To phone number", "To phone number"),
        new(BodyParameterKey, "Body", "Email body")
    ];

    public ParameterValidationResult CanInvoke(List<ParameterValue> invocationValues)
    {
        var errors = new List<ParameterValidationError>();

        // To phone number
        var toPhoneNumber = invocationValues.GetValue(ToPhoneNumberParameterKey);

        if (string.IsNullOrWhiteSpace(toPhoneNumber))
        {
            errors.Add(ErrToPhoneNumberMissing);
        }

        // Body
        var body = invocationValues.GetValue(BodyParameterKey);

        if (string.IsNullOrWhiteSpace(body))
        {
            errors.Add(ErrBodyMissing);
        }

        if (body is { Length: > MaxBodyLength })
        {
            errors.Add(ErrBodyMaxLength);
        }

        return ParameterValidationResult.For(errors);
    }
}