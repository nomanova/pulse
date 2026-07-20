using Pulse.Api.Client.Common;
using Pulse.Api.Ctrl.Contract.Applications;
using Pulse.App.Dto.Applications;
using Pulse.App.Dto.Common;

namespace Pulse.Api.Ctrl.Client.Applications;

public interface IApplicationsService
{
    Task<ApiDataResult<IdentityDto>> Create(CreateApplicationRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResult> Delete(DeleteApplicationRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiDataResult<ApplicationDto>> Fetch(FetchApplicationRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiDataResult<PagedSearchResultDto<ApplicationDto>>> Search(SearchApplicationsRequest request,
        CancellationToken cancellationToken = default);
}