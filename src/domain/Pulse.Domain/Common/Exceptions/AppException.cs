using System;

namespace Pulse.Domain.Common.Exceptions;

public class AppException : Exception
{
    protected AppException()
    {
    }
    
    public AppException(string message) : base(message)
    {
    }

    public AppException(string message, Exception inner) : base(message, inner)
    {
    }
}