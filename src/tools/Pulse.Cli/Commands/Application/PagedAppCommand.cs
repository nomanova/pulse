using System.Threading;
using System.Threading.Tasks;
using Pulse.Api.Client.Common;
using Pulse.Api.Ctrl.Client;
using Pulse.Api.Ctrl.Contract;
using Pulse.App.Dto.Applications;
using Pulse.App.Dto.Common;
using Pulse.Cli.Models;
using Pulse.Cli.Services;
using Spectre.Console;

namespace Pulse.Cli.Commands.Application;

public abstract class PagedAppCommand : PagedCommand<SearchApplicationsRequest, ApplicationDto>
{
    private readonly IConfigService _configService;
    private readonly ICtrlApiClient _ctrlApiClient;
    
    protected PagedAppCommand(
        IAnsiConsole console, 
        IConfigService configService,
        ICtrlApiClient ctrlApiClient) : base(console)
    {
        _configService = configService;
        _ctrlApiClient = ctrlApiClient;
    }

    protected override async Task<ApiDataResult<PagedSearchResultDto<ApplicationDto>>> Search(
        SearchApplicationsRequest request, CancellationToken cancellationToken)
    {
        return await _ctrlApiClient.Applications.Search(request, cancellationToken);
    }

    protected override SearchApplicationsRequest GetRequest()
    {
        var config = _configService.Load();
        config.AssertOrganization();
        
        return new SearchApplicationsRequest
        {
            OrganizationId = config.Context.Organization!.Id
        };
    }
}