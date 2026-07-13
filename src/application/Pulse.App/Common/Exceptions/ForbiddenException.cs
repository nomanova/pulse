using System;
using Pulse.Domain.Common.Exceptions;

namespace Pulse.App.Common.Exceptions;

public sealed class ForbiddenException : AppException
{
    public ForbiddenException()
    {
    }

    public ForbiddenException(string message) : base(message)
    {
    }

    public ForbiddenException(string message, Exception inner) : base(message, inner)
    {
    }
}