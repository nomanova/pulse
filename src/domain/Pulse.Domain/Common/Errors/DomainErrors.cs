using ErrorOr;

namespace Pulse.Domain.Common.Errors;

public static partial class DomainErrors
{
    private static Error TooShort(string code, string property, uint minLength)
    {
        return Error.Validation(code, $"{property} should be at least {minLength} characters");
    }
    
    private static Error TooLong(string code, string property, uint maxLength)
    {
        return Error.Validation(code, $"{property} should not exceed {maxLength} characters");
    }

    private static Error Required(string code, string property)
    {
        return Error.Validation(code, $"{property} is required");
    }
}