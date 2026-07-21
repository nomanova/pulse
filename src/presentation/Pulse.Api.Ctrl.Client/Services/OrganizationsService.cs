using Pulse.Api.Client;
using Pulse.Api.Client.Common;
using Pulse.Api.Client.Services;
using Pulse.Api.Ctrl.Client.Services.Interfaces;
using Pulse.Api.Ctrl.Contract.Organizations;
using Pulse.App.Dto.Common;

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
}