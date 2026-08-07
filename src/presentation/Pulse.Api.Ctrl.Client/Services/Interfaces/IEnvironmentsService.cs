using System.Threading;
using System.Threading.Tasks;
using Pulse.Api.Client.Common;
using Pulse.Api.Ctrl.Contract;
using Pulse.App.Dto.Common;
using Pulse.App.Dto.Environments;

namespace Pulse.Api.Ctrl.Client.Services.Interfaces;

public interface IEnvironmentsService
{
    Task<ApiDataResult<IdentityDto>> Add(AddEnvironmentRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResult> Remove(RemoveEnvironmentRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiDataResult<EnvironmentDto>> Fetch(FetchEnvironmentRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiDataResult<PagedSearchResultDto<EnvironmentDto>>> Search(SearchEnvironmentsRequest request,
        CancellationToken cancellationToken = default);
}