using ErrorOr;
using System.Linq;
using FluentValidation;
using Pulse.Domain.Common.Errors;

namespace Pulse.Domain.Common.Models.ValueObjects;

/**
 * Each object key in the system adheres to the RFC 1123 label name rules:
 * - A label may start with a letter or digit
 * - Must still end with a letter or digit
 * - Interior characters: letters, digits, or hyphens
 * - Max length: 63 characters
 * With the more restrictive addition that all letters must be lowercase.
 */
public sealed record ObjectKey
{
    public const int MaxLength = 61;
    private const string AllowedCharacters = "abcdefghijklmnopqrstuvwxyz0123456789-";

    /**
     * Original value as provided by the user.
     * Used to determine if the key is unique (unambiguous lookup).
     * E.g., acme-hq007
     */
    public string Value { get; } = null!;

    /*
     * The normalized value is the key with only alphanumeric characters.
     * Used when searching by key.
     * E.g., acmehq
     */
    public string NormalizedValue { get; } = null!;

    private ObjectKey()
    {
    }

    private ObjectKey(string value, string normalizedValue)
    {
        Value = value;
        NormalizedValue = normalizedValue;
    }

    public static ErrorOr<ObjectKey> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return DomainErrors.ObjectKey.Required;
        }

        var normalizedValue = value.AsNormalizedObjectKey();
        var key = new ObjectKey(value, normalizedValue);

        var validator = new KeyValidator();
        var result = validator.Validate(key);

        return result.ToErrorOr(key);
    }

    public override string ToString()
    {
        return Value;
    }

    private sealed class KeyValidator : AbstractValidator<ObjectKey>
    {
        public KeyValidator()
        {
            RuleFor(model => model.Value).NotEmpty()
                .WithError(DomainErrors.ObjectKey.Required);
            RuleFor(model => model.Value).Must(value => value.All(c => AllowedCharacters.Contains(c)))
                .WithError(DomainErrors.ObjectKey.ForbiddenCharacter);
            RuleFor(model => model.Value).Matches("^[a-z0-9]([-a-z0-9]*[a-z0-9])?$")
                .WithError(DomainErrors.ObjectKey.InvalidFormat);
            RuleFor(model => model.Value).MaximumLength(MaxLength)
                .WithError(DomainErrors.ObjectKey.TooLong);
        }
    }
}