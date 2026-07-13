using ErrorOr;
using System.Linq;
using FluentValidation;
using Pulse.Domain.Common.Errors;

namespace Pulse.Domain.Common.Models.ValueObjects;

/**
 * Each object name in the system adheres to the RFC 1123 label name rules:
 * - A label may start with a letter or digit
 * - Must still end with a letter or digit
 * - Interior characters: letters, digits, or hyphens
 * - Max length: 63 characters
 * With the more restrictive addition that all letters must be lowercase.
 */
public record ObjectName
{
    public const int MaxLength = 61;
    private const string AllowedCharacters = "abcdefghijklmnopqrstuvwxyz0123456789-";

    /**
     * Original value as provided by the user.
     * Used to determine if the name is unique (unambiguous lookup).
     * E.g., acme-hq
     */
    public string Value { get; } = null!;

    /*
     * The normalized value is the name with only alphanumeric characters.
     * Used when searching by name.
     * E.g., acmehq
     */
    public string NormalizedValue { get; } = null!;

    private ObjectName()
    {
    }

    private ObjectName(string value, string normalizedValue)
    {
        Value = value;
        NormalizedValue = normalizedValue;
    }

    public static ErrorOr<ObjectName> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return DomainErrors.ObjectName.Required;
        }

        var normalizedValue = Normalize(value);
        var name = new ObjectName(value, normalizedValue);

        var validator = new NameValidator();
        var result = validator.Validate(name);

        return result.ToErrorOr(name);
    }

    public override string ToString()
    {
        return Value;
    }

    private static string Normalize(string? value)
    {
        return value is null ? "" : value.Normalize().Replace("-", "");
    }

    private sealed class NameValidator : AbstractValidator<ObjectName>
    {
        public NameValidator()
        {
            RuleFor(model => model.Value).NotEmpty()
                .WithError(DomainErrors.ObjectName.Required);
            RuleFor(model => model.Value).Must(value => value.All(c => AllowedCharacters.Contains(c)))
                .WithError(DomainErrors.ObjectName.ForbiddenCharacter);
            RuleFor(model => model.Value).Matches("^[a-z0-9]([-a-z0-9]*[a-z0-9])?$")
                .WithError(DomainErrors.ObjectName.InvalidFormat);
            RuleFor(model => model.Value).MaximumLength(MaxLength)
                .WithError(DomainErrors.ObjectName.TooLong);
        }
    }
}