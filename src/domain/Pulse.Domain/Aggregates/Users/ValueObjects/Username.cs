using System.Linq;
using ErrorOr;
using FluentValidation;
using Pulse.Domain.Common.Errors;

namespace Pulse.Domain.Aggregates.Users.ValueObjects;

public sealed record Username
{
    private const string AllowedCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.";
    public const int MaxLength = 50;

    /**
     * Original value as provided by the user.
     * Used to determine if the username is unique (unambiguous lookup).
     * The username is case-sensitive (Admin and admin are different).
     */
    public string Value { get; } = null!;

    /*
     * The normalized value is the username with all characters converted to lower case.
     * Used when searching for users by username.
     */
    public string NormalizedValue { get; } = null!;

    private Username()
    {
    }

    private Username(string value, string normalizedValue)
    {
        Value = value;
        NormalizedValue = normalizedValue;
    }

    public static ErrorOr<Username> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return BusinessErrors.User.UsernameRequired;
        }

        var normalizedValue = value.Normalize().ToLowerInvariant();
        var username = new Username(value, normalizedValue);

        var validator = new UsernameValidator();
        var result = validator.Validate(username);

        return result.ToErrorOr(username);
    }

    public override string ToString()
    {
        return Value;
    }

    private sealed class UsernameValidator : AbstractValidator<Username>
    {
        public UsernameValidator()
        {
            RuleFor(model => model.Value).NotEmpty()
                .WithError(BusinessErrors.User.UsernameRequired);
            RuleFor(model => model.Value).Must(value => value.All(c => AllowedCharacters.Contains(c)))
                .WithError(BusinessErrors.User.UsernameForbiddenCharacter);
            RuleFor(model => model.Value).Matches("^[a-zA-Z0-9]([a-zA-Z0-9-._]*[a-zA-Z0-9])?$")
                .WithError(BusinessErrors.User.UsernameInvalidFormat);
            RuleFor(model => model.Value).MaximumLength(MaxLength)
                .WithError(BusinessErrors.User.UsernameTooLong);
        }
    }
}