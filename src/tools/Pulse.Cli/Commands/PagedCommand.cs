using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Pulse.Api.Client.Common;
using Pulse.Api.Shared.Contract;
using Pulse.App.Dto.Common;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Pulse.Cli.Commands;

public abstract class PagedCommand<T, Ts> : AsyncCommand<PagedCommandSettings> where T :
    PagedSearchRequest
    where Ts : IdentityDto
{
    private readonly IAnsiConsole _console;

    protected PagedCommand(IAnsiConsole console)
    {
        _console = console;
    }

    protected abstract Task<ApiDataResult<PagedSearchResultDto<Ts>>> Search(
        T request, CancellationToken cancellationToken);

    protected abstract T GetRequest();

    protected async Task<(string? lastId, List<Ts>? entities)> Fetch(
        PagedCommandSettings settings, CancellationToken cancellationToken)
    {
        settings.Assert();
        var request = GetRequest();

        request = request with
        {
            Query = settings.Query,
            PageSize = settings.Limit
        };
        
        List<Ts>? entities = null;
        string? lastId = null;

        if (settings.All)
        {
            entities = await FetchAll(request, cancellationToken);
        }
        else
        {
            request = request with { LastId = settings.Cursor };
            var result = await FetchPage(request, cancellationToken);

            if (result != null)
            {
                lastId = result.HasNext ? result.Entities[^1].Id : null;
                entities = result.Entities.ToList();
            }
        }

        return (lastId, entities);
    }

    private async Task<List<Ts>?> FetchAll(
        T request, CancellationToken cancellationToken)
    {
        var entities = new List<Ts>();

        string? lastId = null;

        do
        {
            request = request with { LastId = lastId };
            var result = await FetchPage(request, cancellationToken);

            if (result is null)
            {
                return null;
            }

            entities.AddRange(result.Entities);

            if (!result.HasNext)
            {
                break;
            }

            lastId = result.Entities[^1].Id;
        } while (true);

        return entities;
    }

    private async Task<PagedSearchResultDto<Ts>?> FetchPage(
        T request, CancellationToken cancellationToken)
    {
        var result = await Search(request, cancellationToken);

        if (!result.Success)
        {
            _console.WriteProblem(result.Problem, result.StatusCode);
            return null;
        }

        return result.Data;
    }
}