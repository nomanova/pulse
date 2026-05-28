using ErrorOr;

namespace Pulse.Domain.Common.Errors;

public static partial class DomainErrors
{
    public static class Text
    {
        public static Error TooShort(string property, uint minLength)
        {
            return Error.Validation(
                $"Text.TooShort.{property}", $"{property} should be at least {minLength} characters");
        }

        public static Error TooLong(string property, uint maxLength)
        {
            return Error.Validation(
                $"Text.TooLong.{property}", $"{property} should not exceed {maxLength} characters");
        }

        public static Error Required(string property)
        {
            return Error.Validation(
                $"Text.Required.{property}", $"{property} is required");
        }
    }
}