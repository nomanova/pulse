using System.Threading;
using System.Threading.Tasks;
using Pulse.Api.Client.Common;
using Pulse.Api.Ctrl.Contract;
using Pulse.App.Dto.Common;
using Pulse.App.Dto.Connections;

namespace Pulse.Api.Ctrl.Client.Services.Interfaces;

public interface IConnectionsService
{
    Task<ApiDataResult<IdentityDto>> Add(AddConnectionRequest request,
        CancellationToken cancellationToken = default);
    
    Task<ApiResult> Remove(RemoveConnectionRequest request,
        CancellationToken cancellationToken = default);
    
    Task<ApiDataResult<ConnectionDto>> Fetch(FetchConnectionRequest request,
        CancellationToken cancellationToken = default);
    
    Task<ApiDataResult<PagedSearchResultDto<ConnectionDto>>> Search(SearchConnectionsRequest request,
        CancellationToken cancellationToken = default);
}