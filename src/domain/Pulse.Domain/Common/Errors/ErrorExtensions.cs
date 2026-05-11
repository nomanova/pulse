using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ErrorOr;
using Pulse.Domain.Common.Exceptions;

namespace Pulse.Domain.Common.Errors;

public static class ErrorExtensions
{
    public static void Assert(this Error error, Func<bool> validate)
    {
        var isValid = validate();

        if (!isValid)
        {
            error.Throw();
        }
    }
    
    public static T Assert<T>(this ErrorOr<T> result)
    {
        if (result.IsError)
        {
            var errors = result.Errors;
            errors.Throw();
        }

        return result.Value;
    }
    
    public static string Serialize(this Error error)
    {
        return $"{error.Code} - {error.Description}";
    }
    
    public static string Serialize(this List<Error> errors)
    {
        var sb = new StringBuilder();
        
        foreach (var error in errors)
        {
            sb.AppendLine(error.Serialize());
        }
        
        return sb.ToString();
    }
    
    private static void Throw(this Error error)
    {
        throw new DomainException([error]);
    }
    
    private static void Throw(this IEnumerable<Error> errors)
    {
        throw new DomainException(errors.ToList());
    }
}