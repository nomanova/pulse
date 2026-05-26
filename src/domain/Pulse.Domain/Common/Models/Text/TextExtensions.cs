using ErrorOr;
using Pulse.Domain.Common.Errors;

namespace Pulse.Domain.Common.Models.Text;

public static class TextExtensions
{
    private const string NameProperty = "Name";

    extension(string? value)
    {
        public ErrorOr<string> AsName(string? property = null)
        {
            return value.AsText(property ?? NameProperty, Constants.DefaultNameMaxLength);
        }

        private ErrorOr<string> AsText(string property,
            uint maxLength = Constants.DefaultTextMaxLength,
            uint? minLength = null)
        {
            var validator = new TextValidator(property, maxLength, minLength);
            var result = validator.Validate(value);
            return result.ToErrorOr(value!);
        }

        public ErrorOr<string?> AsOptionalText(string property,
            uint maxLength = Constants.DefaultTextMaxLength,
            uint? minLength = null)
        {
            if (string.IsNullOrEmpty(value))
            {
                // ReSharper disable once PreferConcreteValueOverDefault
                return default(string);
            }

            return value.AsText(property, maxLength, minLength)!;
        }
    }
}