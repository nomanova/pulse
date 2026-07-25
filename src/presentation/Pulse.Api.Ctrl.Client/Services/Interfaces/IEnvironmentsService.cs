using Pulse.Api.Client.Common;
using Pulse.Api.Ctrl.Contract.Environments;
using Pulse.App.Dto.Common;
using Pulse.App.Dto.Environments;

namespace Pulse.Api.Ctrl.Client.Services.Interfaces;

public interface IEnvironmentsService
{
    Task<ApiDataResult<IdentityDto>> Create(CreateEnvironmentRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResult> Delete(DeleteEnvironmentRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiDataResult<EnvironmentDto>> Fetch(FetchEnvironmentRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiDataResult<PagedSearchResultDto<EnvironmentDto>>> Search(SearchEnvironmentsRequest request,
        CancellationToken cancellationToken = default);
}