using System.Threading;
using System.Threading.Tasks;
using Pulse.Api.Client.Common;
using Pulse.Api.Ctrl.Client;
using Pulse.Api.Shared.Contract;
using Pulse.App.Dto.Common;
using Pulse.App.Dto.Organizations;
using Spectre.Console;

namespace Pulse.Cli.Commands.Organization;

public abstract class PagedOrgCommand : PagedCommand<PagedSearchRequest, OrganizationDto>
{
    private readonly ICtrlApiClient _ctrlApiClient;

    protected PagedOrgCommand(
        IAnsiConsole console,
        ICtrlApiClient ctrlApiClient) : base(console)
    {
        _ctrlApiClient = ctrlApiClient;
    }

    protected override async Task<ApiDataResult<PagedSearchResultDto<OrganizationDto>>> Search(
        PagedSearchRequest request, CancellationToken cancellationToken)
    {
        return await _ctrlApiClient.Organizations.Search(request, cancellationToken);
    }

    protected override PagedSearchRequest GetRequest()
    {
        return new PagedSearchRequest();
    }
}