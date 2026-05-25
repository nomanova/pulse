using System;
using System.Collections.Generic;
using System.Linq;
using ErrorOr;
using Pulse.Api.Shared.Contract;

namespace Pulse.Api.Shared;

public static class ErrorExtensions
{
    public static Problem AsProblem(this Error error,
        List<ValidationError>? validationErrors = null, Exception? ex = null)
    {
        return new Problem
        {
            Code = error.Code,
            Description = error.Description,
            ValidationErrors = validationErrors,
            Trace = ex == null ? null : $"{ex.Message} {ex.StackTrace}"
        };
    }

    public static Problem AsProblem(this IEnumerable<Error> errors, Exception? ex = null)
    {
        var validationErrors = errors
            .Select(error => new ValidationError(error.Code, error.Description))
            .ToList();
        
        return Error.Validation().AsProblem(validationErrors, ex);
    }
}