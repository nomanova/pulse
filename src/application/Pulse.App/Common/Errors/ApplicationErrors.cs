using ErrorOr;

namespace Pulse.App.Common.Errors;

public static partial class ApplicationErrors
{
    public static readonly Error NameInUse =
        Error.Validation("General.NameInUse", "Name is in use");
    
    public static readonly Error OrderBy =
        Error.Validation("General.OrderBy", "Cannot order by requested property");
    
    public static Error NotFound(string entity)
    {
        return Error.NotFound(
            code: $"{entity}.NotFound",
            description: $"{entity} was not found");
    }
}