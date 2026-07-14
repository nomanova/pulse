using System;

namespace Pulse.Cli;

public class CliException : Exception
{
    protected CliException()
    {
    }
    
    public CliException(string message) : base(message)
    {
    }

    public CliException(string message, Exception inner) : base(message, inner)
    {
    }
}