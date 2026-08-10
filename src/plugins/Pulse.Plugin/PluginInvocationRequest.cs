using System.Collections.Generic;

namespace Pulse.Plugin;

public sealed record PluginInvocationRequest(
    IReadOnlyDictionary<string, string> Parameters
);