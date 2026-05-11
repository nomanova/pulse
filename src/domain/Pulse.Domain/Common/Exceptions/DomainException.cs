using System;
using System.Collections.Generic;
using ErrorOr;
using Pulse.Domain.Common.Errors;

namespace Pulse.Domain.Common.Exceptions;

public sealed class DomainException : AppException
{
    public List<Error> Errors { get; }

    public DomainException(List<Error> errors) : base(errors.Serialize())
    {
        Errors = errors;
    }

    public DomainException(List<Error> errors, Exception inner) : base(errors.Serialize(), inner)
    {
        Errors = errors;
    }
}