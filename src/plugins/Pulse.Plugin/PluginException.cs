using System;

namespace Pulse.Plugin;

public class PluginException : Exception
{
    public PluginException(string message) : base(message)
    {
    }

    public PluginException(string message, Exception inner) : base(message, inner)
    {
    }
}