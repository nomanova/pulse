using Pulse.Api.Client.Common;
using Pulse.Api.Ctrl.Contract.Organizations;
using Pulse.Api.Shared.Contract;
using Pulse.App.Dto.Common;
using Pulse.App.Dto.Organizations;

namespace Pulse.Api.Ctrl.Client.Services.Interfaces;

public interface IOrganizationsService
{
    Task<ApiDataResult<IdentityDto>> Create(CreateOrganizationRequest request,
        CancellationToken cancellationToken = default);
    
    Task<ApiResult> Delete(DeleteOrganizationRequest request,
        CancellationToken cancellationToken = default);
    
    Task<ApiDataResult<OrganizationDto>> Fetch(FetchOrganizationRequest request,
        CancellationToken cancellationToken = default);
    
    Task<ApiDataResult<PagedSearchResultDto<OrganizationDto>>> Search(PagedSearchRequest request,
        CancellationToken cancellationToken = default);
}