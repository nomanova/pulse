using System.Threading.Tasks;
using Pulse.Api.Client.Common;

namespace Pulse.Cli.Services;

public class ApiEndpointProvider : IApiEndpointProvider
{
    private readonly IConfigService _configService;

    public ApiEndpointProvider(IConfigService configService)
    {
        _configService = configService;
    }

    public Task<string?> GetApiEndpoint()
    {
        var config = _configService.Load();
        var server = config.CurrentServer();

        return Task.FromResult(server?.Url);
    }
}