using ErrorOr;
using FluentValidation;
using Pulse.Domain.Common.Errors;

namespace Pulse.Domain.Aggregates.Users.ValueObjects;

public sealed record EmailAddress
{
    public const int MaxLength = 250;

    public static string Normalized(string input)
    {
        return input.Trim().ToLowerInvariant();
    }

    public string Value { get; private set; } = null!;

    public bool IsValidated { get; private set; }

    private EmailAddress()
    {
    }

    private EmailAddress(string value)
    {
        Value = value;
    }

    public static ErrorOr<EmailAddress> Create(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return BusinessErrors.User.EmailAddressRequired;
        }

        var normalizedValue = Normalized(value);
        var emailAddress = new EmailAddress(normalizedValue);

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
            RuleFor(model => model.Value).NotEmpty().WithError(BusinessErrors.User.EmailAddressRequired);
            RuleFor(model => model.Value).EmailAddress().WithError(BusinessErrors.User.EmailAddressInvalid);
            RuleFor(model => model.Value).MaximumLength(MaxLength).WithError(BusinessErrors.User.EmailAddressTooLong);
        }
    }
}