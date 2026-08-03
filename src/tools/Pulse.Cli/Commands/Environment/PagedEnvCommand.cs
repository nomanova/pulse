using System.Threading;
using System.Threading.Tasks;
using Pulse.Api.Client.Common;
using Pulse.Api.Ctrl.Client;
using Pulse.Api.Ctrl.Contract;
using Pulse.App.Dto.Common;
using Pulse.App.Dto.Environments;
using Pulse.Cli.Models;
using Pulse.Cli.Services;
using Spectre.Console;

namespace Pulse.Cli.Commands.Environment;

public abstract class PagedEnvCommand : PagedCommand<SearchEnvironmentsRequest, EnvironmentDto>
{
    private readonly IConfigService _configService;
    private readonly ICtrlApiClient _ctrlApiClient;
    
    protected PagedEnvCommand(
        IAnsiConsole console, 
        IConfigService configService, 
        ICtrlApiClient ctrlApiClient) : base(console)
    {
        _configService = configService;
        _ctrlApiClient = ctrlApiClient;
    }

    protected override async Task<ApiDataResult<PagedSearchResultDto<EnvironmentDto>>> Search(
        SearchEnvironmentsRequest request, CancellationToken cancellationToken)
    {
        return await _ctrlApiClient.Environments.Search(request, cancellationToken);
    }

    protected override SearchEnvironmentsRequest GetRequest()
    {
        var config = _configService.Load();
        config.AssertApplication();
        
        return new SearchEnvironmentsRequest
        {
            ApplicationId = config.Context.Application!.Id
        };
    }
}