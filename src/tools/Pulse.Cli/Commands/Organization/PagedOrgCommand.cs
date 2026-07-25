using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Pulse.Api.Ctrl.Client;
using Pulse.Api.Shared.Contract;
using Pulse.App.Dto.Common;
using Pulse.App.Dto.Organizations;
using Spectre.Console;

namespace Pulse.Cli.Commands.Organization;

public abstract class PagedOrgCommand : PagedCommand
{
    private readonly IAnsiConsole _console;
    private readonly ICtrlApiClient _ctrlApiClient;

    protected PagedOrgCommand(
        IAnsiConsole console,
        ICtrlApiClient ctrlApiClient)
    {
        _console = console;
        _ctrlApiClient = ctrlApiClient;
    }

    protected async Task<(string? lastId, List<OrganizationDto>? entities)> Fetch(
        PagedCommandSettings settings, CancellationToken cancellationToken)
    {
        settings.Assert();
        var limit = settings.Limit;

        List<OrganizationDto>? organizations = null;
        string? lastId = null;

        if (settings.All)
        {
            organizations = await FetchAll(settings.Query, limit, cancellationToken);
        }
        else
        {
            var result = await FetchPage(settings.Query, limit, settings.Cursor, cancellationToken);

            if (result != null)
            {
                lastId = result.HasNext ? result.Entities[^1].Id : null;
                organizations = result.Entities.ToList();
            }
        }

        return (lastId, organizations);
    }

    private async Task<List<OrganizationDto>?> FetchAll(
        string? query, uint limit, CancellationToken cancellationToken)
    {
        var organizations = new List<OrganizationDto>();

        string? lastId = null;

        do
        {
            var result = await FetchPage(query, limit, lastId, cancellationToken);

            if (result is null)
            {
                return null;
            }

            organizations.AddRange(result.Entities);

            if (!result.HasNext)
            {
                break;
            }

            lastId = result.Entities[^1].Id;
        } while (true);

        return organizations;
    }

    private async Task<PagedSearchResultDto<OrganizationDto>?> FetchPage(
        string? query, uint limit, string? lastId, CancellationToken cancellationToken)
    {
        var request = new PagedSearchRequest
        {
            Query = query,
            PageSize = limit,
            LastId = lastId
        };

        var result = await _ctrlApiClient.Organizations.Search(request, cancellationToken);

        if (!result.Success)
        {
            _console.WriteProblem(result.Problem, result.StatusCode);
            return null;
        }

        return result.Data;
    }
}