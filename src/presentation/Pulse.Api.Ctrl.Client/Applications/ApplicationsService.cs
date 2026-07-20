using Pulse.Api.Client;
using Pulse.Api.Client.Common;
using Pulse.Api.Client.Services;
using Pulse.Api.Ctrl.Contract.Applications;
using Pulse.App.Dto.Applications;
using Pulse.App.Dto.Common;

namespace Pulse.Api.Ctrl.Client.Applications;

public sealed class ApplicationsService : BaseService, IApplicationsService
{
    private const string BasePath = "/api/ctrl/v1/applications";

    public ApplicationsService(IEndpointProvider? endpointProvider, ApiHttpClient? httpClient)
        : base(endpointProvider, httpClient, BasePath)
    {
    }

    public async Task<ApiDataResult<IdentityDto>> Create(CreateApplicationRequest request,
        CancellationToken cancellationToken = default)
    {
        const string url = $"{BasePath}/create";
        return await SendForDataAsync<IdentityDto>(HttpMethod.Post, url, request, cancellationToken);
    }

    public async Task<ApiResult> Delete(DeleteApplicationRequest request, CancellationToken cancellationToken = default)
    {
        const string url = $"{BasePath}/delete";
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