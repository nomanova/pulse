using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Pulse.Api.Client;
using Pulse.Api.Client.Common;
using Pulse.Api.Client.Services;
using Pulse.Api.Ctrl.Client.Services.Interfaces;
using Pulse.Api.Ctrl.Contract;
using Pulse.App.Dto.Common;
using Pulse.App.Dto.Connections;

namespace Pulse.Api.Ctrl.Client.Services;

public sealed class ConnectionsService : BaseService, IConnectionsService
{
    private const string BasePath = "/api/ctrl/v1/connections";

    public ConnectionsService(
        IEndpointProvider? endpointProvider, ITokenProvider? tokenProvider, ApiHttpClient? httpClient)
        : base(endpointProvider, tokenProvider, httpClient)
    {
    }

    public async Task<ApiDataResult<IdentityDto>> Add(AddConnectionRequest request,
        CancellationToken cancellationToken = default)
    {
        const string url = $"{BasePath}/add";
        return await SendForDataAsync<IdentityDto>(HttpMethod.Post, url, request, cancellationToken);
    }

    public async Task<ApiResult> Remove(RemoveConnectionRequest request, CancellationToken cancellationToken = default)
    {
        const string url = $"{BasePath}/remove";
        return await SendAsync(HttpMethod.Post, url, request, cancellationToken);
    }

    public async Task<ApiDataResult<ConnectionDto>> Fetch(FetchConnectionRequest request,
        CancellationToken cancellationToken = default)
    {
        const string url = $"{BasePath}/fetch";
        return await SendForDataAsync<ConnectionDto>(HttpMethod.Post, url, request, cancellationToken);
    }

    public async Task<ApiDataResult<PagedSearchResultDto<ConnectionDto>>> Search(SearchConnectionsRequest request,
        CancellationToken cancellationToken = default)
    {
        const string url = $"{BasePath}/search";
        return await SendForDataAsync<PagedSearchResultDto<ConnectionDto>>(HttpMethod.Post, url, request,
            cancellationToken);
    }
}