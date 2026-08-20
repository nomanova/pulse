using System.Net.Http;

namespace Pulse.Plugin;

public interface IPluginHostContext
{
    HttpClient HttpClient();
    
    void Log(PluginLogLevel level, string message);
}