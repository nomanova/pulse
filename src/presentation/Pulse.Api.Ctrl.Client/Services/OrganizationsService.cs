using Pulse.Api.Client;
using Pulse.Api.Client.Common;
using Pulse.Api.Client.Services;
using Pulse.Api.Ctrl.Client.Services.Interfaces;
using Pulse.Api.Ctrl.Contract.Organizations;
using Pulse.Api.Shared.Contract;
using Pulse.App.Dto.Common;
using Pulse.App.Dto.Organizations;

namespace Pulse.Api.Ctrl.Client.Services;

public sealed class OrganizationsService : BaseService, IOrganizationsService
{
    private const string BasePath = "/api/ctrl/v1/organizations";
    
    public OrganizationsService(
        IEndpointProvider? endpointProvider, ITokenProvider? tokenProvider, ApiHttpClient? httpClient)
        : base(endpointProvider, tokenProvider, httpClient)
    {
    }

    public async Task<ApiDataResult<IdentityDto>> Create(CreateOrganizationRequest request, CancellationToken cancellationToken = default)
    {
        const string url = $"{BasePath}/create";
        return await SendForDataAsync<IdentityDto>(HttpMethod.Post, url, request, cancellationToken);
    }

    public async Task<ApiResult> Delete(DeleteOrganizationRequest request, CancellationToken cancellationToken = default)
    {
        const string url = $"{BasePath}/delete";
        return await SendAsync(HttpMethod.Post, url, request, cancellationToken);
    }

    public async Task<ApiDataResult<OrganizationDto>> Fetch(FetchOrganizationRequest request, CancellationToken cancellationToken = default)
    {
        const string url = $"{BasePath}/fetch";
        return await SendForDataAsync<OrganizationDto>(HttpMethod.Post, url, request, cancellationToken);
    }

    public async Task<ApiDataResult<PagedSearchResultDto<OrganizationDto>>> Search(PagedSearchRequest request, CancellationToken cancellationToken = default)
    {
        const string url = $"{BasePath}/search";
        return await SendForDataAsync<PagedSearchResultDto<OrganizationDto>>(HttpMethod.Post, url, request,
            cancellationToken);
    }
}