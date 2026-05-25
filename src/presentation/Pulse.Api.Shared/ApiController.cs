using System.Collections.Generic;
using System.Linq;
using System.Net;
using ErrorOr;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pulse.Api.Shared.Contract;
using Throw;

namespace Pulse.Api.Shared;

[ApiController]
public class ApiController : ControllerBase
{
    protected static IActionResult Problem(List<Error> errors)
    {
        errors.ThrowIfNull().IfEmpty();

        if (errors.All(error => error.Type == ErrorType.Validation))
        {
            return ValidationProblem(errors);
        }

        var firstError = errors[0];
        return Problem(firstError);
    }

    private static ObjectResult Problem(Error error)
    {
        var statusCode = error.NumericType switch
        {
            (int)ErrorType.NotFound => StatusCodes.Status404NotFound,
            (int)ErrorType.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };

        var problem = new Problem
        {
            Code = error.Code,
            Description = error.Description
        };

        return new ObjectResult(problem)
        {
            StatusCode = statusCode
        };
    }

    private static ObjectResult ValidationProblem(IEnumerable<Error> errors)
    {
        var problem = errors.AsProblem();

        return new ObjectResult(problem)
        {
            StatusCode = (int)HttpStatusCode.BadRequest
        };
    }
}