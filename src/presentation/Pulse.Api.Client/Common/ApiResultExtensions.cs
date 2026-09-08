namespace Pulse.Api.Client.Common;

public static class ApiResultExtensions
{
    public static bool IsSuccessWithData<T>(this ApiDataResult<T>? result)
    {
        return result is { Success: true } && result.Data != null;
    }
}