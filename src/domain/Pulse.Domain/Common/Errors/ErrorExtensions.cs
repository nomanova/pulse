using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ErrorOr;
using FluentValidation;
using FluentValidation.Results;
using Pulse.Domain.Common.Exceptions;
using Throw;

namespace Pulse.Domain.Common.Errors;

public static class ErrorExtensions
{
    public static T Assert<T>(this ErrorOr<T> result)
    {
        if (result.IsError)
        {
            var errors = result.Errors;
            errors.Throw();
        }

        return result.Value;
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

    public static IRuleBuilderOptions<T, TProperty> WithError<T, TProperty>(
        this IRuleBuilderOptions<T, TProperty> rule, Error? error)
    {
        error.ThrowIfNull();
        return rule.WithErrorCode(error.Value.Code).WithMessage(error.Value.Description);
    }

    extension(ValidationResult validationResult)
    {
        public ErrorOr<T> ToErrorOr<T>(T data)
        {
            validationResult.ThrowIfNull();

            if (validationResult.IsValid)
            {
                return data;
            }

            return validationResult.Errors.ConvertAll(error => Error.Validation(error.ErrorCode, error.ErrorMessage));
        }

        public ErrorOr<T> ToErrorOr<T>()
        {
            validationResult.ThrowIfNull();

            return validationResult.IsValid
                ? throw new ArgumentException(null, nameof(validationResult))
                : validationResult.Errors.ConvertAll(error => Error.Validation(error.ErrorCode, error.ErrorMessage));
        }
    }

    extension(Error error)
    {
        public void Assert(Func<bool> validate)
        {
            var isValid = validate();

            if (!isValid)
            {
                error.Throw();
            }
        }
        
        public void Throw()
        {
            throw new DomainException([error]);
        }
        
        private string Serialize()
        {
            return $"{error.Code} - {error.Description}";
        }
    }

    private static void Throw(this IEnumerable<Error> errors)
    {
        throw new DomainException(errors.ToList());
    }
}