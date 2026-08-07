using System.Threading;
using System.Threading.Tasks;
using Pulse.Api.Client.Common;
using Pulse.Api.Ctrl.Contract;
using Pulse.Api.Shared.Contract;
using Pulse.App.Dto.Common;
using Pulse.App.Dto.Organizations;

namespace Pulse.Api.Ctrl.Client.Services.Interfaces;

public interface IOrganizationsService
{
    Task<ApiDataResult<IdentityDto>> Add(AddOrganizationRequest request,
        CancellationToken cancellationToken = default);
    
    Task<ApiResult> Remove(RemoveOrganizationRequest request,
        CancellationToken cancellationToken = default);
    
    Task<ApiDataResult<OrganizationDto>> Fetch(FetchOrganizationRequest request,
        CancellationToken cancellationToken = default);
    
    Task<ApiDataResult<PagedSearchResultDto<OrganizationDto>>> Search(PagedSearchRequest request,
        CancellationToken cancellationToken = default);
}