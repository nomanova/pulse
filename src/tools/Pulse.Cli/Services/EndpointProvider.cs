using System.Threading.Tasks;
using Pulse.Api.Client.Common;

namespace Pulse.Cli.Services;

public class EndpointProvider : IEndpointProvider
{
    private readonly IConfigService _configService;

    public EndpointProvider(IConfigService configService)
    {
        _configService = configService;
    }

    public Task<string?> Get()
    {
        var config = _configService.Load();
        var server = config.CurrentServer();

        return Task.FromResult(server?.Url);
    }
}