using System;
using Pulse.Domain.Common.Exceptions;

namespace Pulse.App.Common.Exceptions;

public class UnauthorizedException : AppException
{
    public UnauthorizedException()
    {
    }

    public UnauthorizedException(string message) : base(message)
    {
    }

    public UnauthorizedException(string message, Exception inner) : base(message, inner)
    {
    }
}