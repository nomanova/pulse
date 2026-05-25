using System.Linq;
using ErrorOr;
using FluentValidation;
using Pulse.Domain.Common.Errors;

namespace Pulse.Domain.Aggregates.Users.ValueObjects;

public sealed record EmailAddress
{
    // Not accepting email addresses with diacritics
    private const string AllowedCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    public const int MaxLength = 250;

    /**
     * Original value as provided by the user.
     */
    public string Value { get; } = null!;

    /*
     * Used to determine if an email address is unique (unambiguous lookup).
     * The email address is case-insensitive (Admin@mail.com and admin@mail.com are identical).
     * The normalized value is the email address with all characters converted to lower case.
     * Also used when searching for users by email address.
     */
    public string NormalizedValue { get; } = null!;

    public bool IsConfirmed { get; private set; }

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

        var normalizedValue = value.Normalize().ToLowerInvariant();
        var emailAddress = new EmailAddress(value, normalizedValue);

        var validator = new EmailAddressValidator();
        var result = validator.Validate(emailAddress);

        return result.ToErrorOr(emailAddress);
    }

    internal void SetConfirmed()
    {
        IsConfirmed = true;
    }

    public override string ToString()
    {
        return Value;
    }

    private sealed class EmailAddressValidator : AbstractValidator<EmailAddress>
    {
        public EmailAddressValidator()
        {
            RuleFor(model => model.Value).NotEmpty()
                .WithError(BusinessErrors.User.EmailAddressRequired);
            RuleFor(model => model.Value).Must(value => value.All(c => AllowedCharacters.Contains(c)))
                .WithError(BusinessErrors.User.EmailAddressForbiddenCharacter);
            RuleFor(model => model.Value).EmailAddress()
                .WithError(BusinessErrors.User.EmailAddressInvalidFormat);
            RuleFor(model => model.Value).MaximumLength(MaxLength)
                .WithError(BusinessErrors.User.EmailAddressTooLong);
        }
    }
}