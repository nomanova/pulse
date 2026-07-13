using System;
using Pulse.Domain.Common.Exceptions;

namespace Pulse.App.Common.Exceptions;

public sealed class ContextException : AppException
{
    public ContextException()
    {
    }

    public ContextException(string message) : base(message)
    {
    }

    public ContextException(string message, Exception inner) : base(message, inner)
    {
    }
}