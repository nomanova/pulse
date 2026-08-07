using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Pulse.Api.Client;
using Pulse.Api.Client.Common;
using Pulse.Api.Client.Services;
using Pulse.Api.Ctrl.Client.Services.Interfaces;
using Pulse.Api.Ctrl.Contract;
using Pulse.App.Dto.Common;
using Pulse.App.Dto.Environments;

namespace Pulse.Api.Ctrl.Client.Services;

public sealed class EnvironmentsService : BaseService, IEnvironmentsService
{
    private const string BasePath = "/api/ctrl/v1/environments";

    public EnvironmentsService(
        IEndpointProvider? endpointProvider, ITokenProvider? tokenProvider, ApiHttpClient? httpClient)
        : base(endpointProvider, tokenProvider, httpClient)
    {
    }

    public async Task<ApiDataResult<IdentityDto>> Add(AddEnvironmentRequest request,
        CancellationToken cancellationToken = default)
    {
        const string url = $"{BasePath}/add";
        return await SendForDataAsync<IdentityDto>(HttpMethod.Post, url, request, cancellationToken);
    }

    public async Task<ApiResult> Remove(RemoveEnvironmentRequest request, CancellationToken cancellationToken = default)
    {
        const string url = $"{BasePath}/remove";
        return await SendAsync(HttpMethod.Post, url, request, cancellationToken);
    }

    public async Task<ApiDataResult<EnvironmentDto>> Fetch(FetchEnvironmentRequest request,
        CancellationToken cancellationToken = default)
    {
        const string url = $"{BasePath}/fetch";
        return await SendForDataAsync<EnvironmentDto>(HttpMethod.Post, url, request, cancellationToken);
    }

    public async Task<ApiDataResult<PagedSearchResultDto<EnvironmentDto>>> Search(SearchEnvironmentsRequest request,
        CancellationToken cancellationToken = default)
    {
        const string url = $"{BasePath}/search";
        return await SendForDataAsync<PagedSearchResultDto<EnvironmentDto>>(HttpMethod.Post, url, request,
            cancellationToken);
    }
}