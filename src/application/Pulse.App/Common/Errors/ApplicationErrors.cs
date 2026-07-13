using ErrorOr;

namespace Pulse.App.Common.Errors;

public static partial class ApplicationErrors
{
    public static Error NotFound(string entity)
    {
        return Error.NotFound(
            code: $"{entity}.NotFound",
            description: $"{entity} was not found.");
    }
}