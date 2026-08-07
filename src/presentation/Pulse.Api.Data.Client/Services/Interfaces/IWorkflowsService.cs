using System.Threading;
using System.Threading.Tasks;
using Pulse.Api.Client.Common;
using Pulse.Api.Data.Contract;
using Pulse.App.Dto.Common;
using Pulse.App.Dto.Workflows;

namespace Pulse.Api.Data.Client.Services.Interfaces;

public interface IWorkflowsService
{
    Task<ApiDataResult<IdentityDto>> Add(AddWorkflowRequest request,
        CancellationToken cancellationToken = default);
    
    Task<ApiResult> Remove(RemoveWorkflowRequest request,
        CancellationToken cancellationToken = default);
    
    Task<ApiDataResult<WorkflowDto>> Fetch(FetchWorkflowRequest request,
        CancellationToken cancellationToken = default);
    
    Task<ApiDataResult<PagedSearchResultDto<WorkflowDto>>> Search(SearchWorkflowsRequest request,
        CancellationToken cancellationToken = default);
}