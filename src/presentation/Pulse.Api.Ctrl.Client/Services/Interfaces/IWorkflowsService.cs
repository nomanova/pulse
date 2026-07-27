using Pulse.Api.Client.Common;
using Pulse.Api.Ctrl.Contract.Workflows;
using Pulse.App.Dto.Common;
using Pulse.App.Dto.Workflows;

namespace Pulse.Api.Ctrl.Client.Services.Interfaces;

public interface IWorkflowsService
{
    Task<ApiDataResult<IdentityDto>> Create(CreateWorkflowRequest request,
        CancellationToken cancellationToken = default);
    
    Task<ApiResult> Delete(DeleteWorkflowRequest request,
        CancellationToken cancellationToken = default);
    
    Task<ApiDataResult<WorkflowDto>> Fetch(FetchWorkflowRequest request,
        CancellationToken cancellationToken = default);
    
    Task<ApiDataResult<PagedSearchResultDto<WorkflowDto>>> Search(SearchWorkflowsRequest request,
        CancellationToken cancellationToken = default);
}