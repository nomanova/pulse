using ErrorOr;
using FluentValidation;
using Pulse.Domain.Common.Errors;

namespace Pulse.Domain.Aggregates.Users.ValueObjects;

public sealed record EmailAddress
{
    public const int MaxLength = 250;

    public static string Normalized(string input)
    {
        return input.Trim().Normalize().ToUpperInvariant();
    }

    public string Value { get; private set; } = null!;

    public string NormalizedValue { get; private set; } = null!;

    public bool IsValidated { get; private set; }

    private EmailAddress()
    {
    }

    private EmailAddress(string value, string normalizedValue)
    {
        Value = value;
        NormalizedValue = normalizedValue;
    }

    public static ErrorOr<EmailAddress> Create(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return BusinessErrors.User.EmailAddressRequired;
        }

        var normalizedValue = Normalized(value);
        var emailAddress = new EmailAddress(value, normalizedValue);

        var validator = new EmailAddressValidator();
        var result = validator.Validate(emailAddress);

        return result.ToErrorOr(emailAddress);
    }

    internal void SetValidated()
    {
        IsValidated = true;
    }

    public override string ToString()
    {
        return Value;
    }

    private sealed class EmailAddressValidator : AbstractValidator<EmailAddress>
    {
        public EmailAddressValidator()
        {
            RuleFor(model => model.NormalizedValue).NotEmpty().WithError(BusinessErrors.User.EmailAddressRequired);
            RuleFor(model => model.NormalizedValue).EmailAddress().WithError(BusinessErrors.User.EmailAddressInvalid);
            RuleFor(model => model.NormalizedValue).MaximumLength(MaxLength)
                .WithError(BusinessErrors.User.EmailAddressTooLong);
        }
    }
}