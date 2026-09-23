using System.Linq;
using System.Net;

namespace Pulse.Api.Client.Common;

public static class ApiResultExtensions
{
    public static bool IsSuccessWithData<T>(this ApiDataResult<T>? result)
    {
        return result is { Success: true } && !Equals(result.Data, default(T));
    }

    public static bool HasValidationError<T>(this ApiDataResult<T>? result, string errorCode)
    {
        return ((ApiResult?)result).HasValidationError(errorCode);
    }
    
    public static bool HasValidationError(this ApiResult? result, string errorCode)
    {
        if (result == null)
        {
            return false;
        }

        if (result.StatusCode != HttpStatusCode.BadRequest)
        {
            return false;
        }

        var validationErrors = result.Problem?.ValidationErrors;

        if (validationErrors == null || !validationErrors.Any())
        {
            return false;
        }

        return validationErrors.FirstOrDefault(validationError => validationError.Code == errorCode) != null;
    }
}