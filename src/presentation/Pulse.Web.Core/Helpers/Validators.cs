using System.Linq;
using System.Text.RegularExpressions;
using FluentValidation;

namespace Pulse.Web.Core.Helpers;

public static partial class Validators
{
    [GeneratedRegex("^[a-z0-9]([-a-z0-9]*[a-z0-9])?$")]
    private static partial Regex Rfc1123Regex();
    
    public static IRuleBuilderOptions<T, string?> ValidName<T>(
        this IRuleBuilder<T, string?> ruleBuilder)
    {
        const int maxLength = 61;
        const string allowedCharacters = "abcdefghijklmnopqrstuvwxyz0123456789-";

        return ruleBuilder
            .NotEmpty()
            .WithMessage("Name must not be empty.")
            .Must(value => string.IsNullOrEmpty(value) || value.All(c => allowedCharacters.Contains(c)))
            .WithMessage("Name contains invalid characters (a-z,0-9 and - only).")
            .Must(value => string.IsNullOrEmpty(value) || Rfc1123Regex().IsMatch(value))
            .WithMessage("Name must start and end with a lowercase letter or number, and may contain hyphens.")
            .MaximumLength(maxLength)
            .WithMessage($"Name must be {maxLength} characters or fewer.");
    }
}