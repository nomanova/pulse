using System.Linq;
using System.Text.RegularExpressions;
using FluentValidation;

namespace Pulse.Web.Core.Common.Validation;

public static partial class Validators
{
    public const int MaxNameLength = 100;
    public const int MaxKeyLength = 61;
    
    [GeneratedRegex("^[a-z0-9]([-a-z0-9]*[a-z0-9])?$")]
    private static partial Regex Rfc1123Regex();

    public static IRuleBuilderOptions<T, string?> ValidName<T>(
        this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty()
            .WithMessage("Name must not be empty.")
            .MaximumLength(MaxNameLength)
            .WithMessage($"Name must be {MaxNameLength} characters or fewer.");
    }

    public static IRuleBuilderOptions<T, string?> ValidKey<T>(
        this IRuleBuilder<T, string?> ruleBuilder)
    {
        const string allowedCharacters = "abcdefghijklmnopqrstuvwxyz0123456789-";

        return ruleBuilder
            .NotEmpty()
            .WithMessage("Key must not be empty.")
            .Must(value => string.IsNullOrEmpty(value) || value.All(c => allowedCharacters.Contains(c)))
            .WithMessage("Key contains invalid characters (a-z,0-9 and - only).")
            .Must(value => string.IsNullOrEmpty(value) || Rfc1123Regex().IsMatch(value))
            .WithMessage("Key must start and end with a lowercase letter or number, and may contain hyphens.")
            .MaximumLength(MaxKeyLength)
            .WithMessage($"Key must be {MaxKeyLength} characters or fewer.");
    }
}