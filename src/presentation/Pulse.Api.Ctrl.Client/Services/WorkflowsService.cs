using Pulse.Api.Client;
using Pulse.Api.Client.Common;
using Pulse.Api.Client.Services;
using Pulse.Api.Ctrl.Client.Services.Interfaces;
using Pulse.Api.Ctrl.Contract.Workflows;
using Pulse.App.Dto.Common;
using Pulse.App.Dto.Workflows;

namespace Pulse.Api.Ctrl.Client.Services;

public sealed class WorkflowsService : BaseService, IWorkflowsService
{
    private const string BasePath = "/api/ctrl/v1/workflows";

    public WorkflowsService(
        IEndpointProvider? endpointProvider, ITokenProvider? tokenProvider, ApiHttpClient? httpClient)
        : base(endpointProvider, tokenProvider, httpClient)
    {
    }

    public async Task<ApiDataResult<IdentityDto>> Create(CreateWorkflowRequest request,
        CancellationToken cancellationToken = default)
    {
        const string url = $"{BasePath}/create";
        return await SendForDataAsync<IdentityDto>(HttpMethod.Post, url, request, cancellationToken);
    }

    public async Task<ApiResult> Delete(DeleteWorkflowRequest request, CancellationToken cancellationToken = default)
    {
        const string url = $"{BasePath}/delete";
        return await SendAsync(HttpMethod.Post, url, request, cancellationToken);
    }

    public async Task<ApiDataResult<WorkflowDto>> Fetch(FetchWorkflowRequest request,
        CancellationToken cancellationToken = default)
    {
        const string url = $"{BasePath}/fetch";
        return await SendForDataAsync<WorkflowDto>(HttpMethod.Post, url, request, cancellationToken);
    }

    public async Task<ApiDataResult<PagedSearchResultDto<WorkflowDto>>> Search(SearchWorkflowsRequest request,
        CancellationToken cancellationToken = default)
    {
        const string url = $"{BasePath}/search";
        return await SendForDataAsync<PagedSearchResultDto<WorkflowDto>>(HttpMethod.Post, url, request,
            cancellationToken);
    }
}