using ErrorOr;
using Pulse.Domain.Common.Errors;

namespace Pulse.Domain.Common.Models.Text;

public static class TextExtensions
{
    private const string NameProperty = "Name";

    public static ErrorOr<string> AsName(this string? value, string? property = null)
    {
        return value.AsText(property ?? NameProperty, Constants.DefaultNameMaxLength);
    }

    private static ErrorOr<string> AsText(
        this string? value,
        string property,
        uint maxLength = Constants.DefaultTextMaxLength,
        uint? minLength = null)
    {
        var validator = new TextValidator(property, maxLength, minLength);
        var result = validator.Validate(value);
        return result.ToErrorOr(value!);
    }
}