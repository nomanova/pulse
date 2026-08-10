using System.Threading;
using System.Threading.Tasks;

namespace Pulse.Plugin.Echo;

public sealed class Plugin : IPlugin
{
    private IPluginHostContext? _hostContext;

    public PluginMetadata Metadata => new(
        Id: "com.nomanova.pulse.plugin.echo",
        DisplayName: "Echo",
        Version: "1.0.0",
        Description: "Logs the input text"
    );

    public Task InitializeAsync(IPluginHostContext hostContext, CancellationToken cancellationToken)
    {
        _hostContext = hostContext;

        return Task.CompletedTask;
    }

    public Task<PluginInvocationResult> InvokeAsync(PluginInvocationRequest request,
        CancellationToken cancellationToken)
    {
        if (_hostContext == null)
        {
            return Task.FromResult(PluginInvocationResult.Fail("Echo: not initialized"));
        }

        if (request.Parameters.TryGetValue("text", out var text))
        {
            _hostContext.Log(PluginLogLevel.Information, $"Echo: {text}");
            return Task.FromResult(PluginInvocationResult.Ok());
        }

        return Task.FromResult(PluginInvocationResult.Fail("Echo: no 'text' parameter provided"));
    }
}