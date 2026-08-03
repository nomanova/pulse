using Pulse.Api.Client.Common;
using Pulse.Api.Ctrl.Contract;
using Pulse.App.Dto.Applications;
using Pulse.App.Dto.Common;

namespace Pulse.Api.Ctrl.Client.Services.Interfaces;

public interface IApplicationsService
{
    Task<ApiDataResult<IdentityDto>> Add(AddApplicationRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResult> Remove(RemoveApplicationRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiDataResult<ApplicationDto>> Fetch(FetchApplicationRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiDataResult<PagedSearchResultDto<ApplicationDto>>> Search(SearchApplicationsRequest request,
        CancellationToken cancellationToken = default);
}