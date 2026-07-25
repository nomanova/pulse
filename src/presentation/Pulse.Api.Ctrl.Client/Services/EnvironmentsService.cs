using Pulse.Api.Client;
using Pulse.Api.Client.Common;
using Pulse.Api.Client.Services;
using Pulse.Api.Ctrl.Client.Services.Interfaces;
using Pulse.Api.Ctrl.Contract.Environments;
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

    public async Task<ApiDataResult<IdentityDto>> Create(CreateEnvironmentRequest request,
        CancellationToken cancellationToken = default)
    {
        const string url = $"{BasePath}/create";
        return await SendForDataAsync<IdentityDto>(HttpMethod.Post, url, request, cancellationToken);
    }

    public async Task<ApiResult> Delete(DeleteEnvironmentRequest request, CancellationToken cancellationToken = default)
    {
        const string url = $"{BasePath}/delete";
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