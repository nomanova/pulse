using System.Collections.Generic;

namespace Pulse.Domain.Channels;

public sealed record ParameterValidationResult
{
    public static ParameterValidationResult Success()
    {
        return new ParameterValidationResult { IsSuccess = true };
    }

    public static ParameterValidationResult Failure(List<ParameterValidationError> errors)
    {
        return new ParameterValidationResult { IsSuccess = false, Errors = errors };
    }

    public static ParameterValidationResult For(List<ParameterValidationError> errors)
    {
        return errors.Count > 0 ? Failure(errors) : Success();
    }

    public bool IsSuccess { get; private init; }

    public IReadOnlyList<ParameterValidationError> Errors { get; private init; } = [];
}