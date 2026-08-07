using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Pulse.Api.Client;
using Pulse.Api.Client.Common;
using Pulse.Api.Client.Services;
using Pulse.Api.Ctrl.Client.Services.Interfaces;
using Pulse.Api.Ctrl.Contract;
using Pulse.App.Dto.Applications;
using Pulse.App.Dto.Common;

namespace Pulse.Api.Ctrl.Client.Services;

public sealed class ApplicationsService : BaseService, IApplicationsService
{
    private const string BasePath = "/api/ctrl/v1/applications";

    public ApplicationsService(
        IEndpointProvider? endpointProvider, ITokenProvider? tokenProvider, ApiHttpClient? httpClient)
        : base(endpointProvider, tokenProvider, httpClient)
    {
    }

    public async Task<ApiDataResult<IdentityDto>> Add(AddApplicationRequest request,
        CancellationToken cancellationToken = default)
    {
        const string url = $"{BasePath}/add";
        return await SendForDataAsync<IdentityDto>(HttpMethod.Post, url, request, cancellationToken);
    }

    public async Task<ApiResult> Remove(RemoveApplicationRequest request, CancellationToken cancellationToken = default)
    {
        const string url = $"{BasePath}/remove";
        return await SendAsync(HttpMethod.Post, url, request, cancellationToken);
    }

    public async Task<ApiDataResult<ApplicationDto>> Fetch(FetchApplicationRequest request,
        CancellationToken cancellationToken = default)
    {
        const string url = $"{BasePath}/fetch";
        return await SendForDataAsync<ApplicationDto>(HttpMethod.Post, url, request, cancellationToken);
    }

    public async Task<ApiDataResult<PagedSearchResultDto<ApplicationDto>>> Search(SearchApplicationsRequest request,
        CancellationToken cancellationToken = default)
    {
        const string url = $"{BasePath}/search";
        return await SendForDataAsync<PagedSearchResultDto<ApplicationDto>>(HttpMethod.Post, url, request,
            cancellationToken);
    }
}