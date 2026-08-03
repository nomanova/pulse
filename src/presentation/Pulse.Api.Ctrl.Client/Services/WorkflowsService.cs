using Pulse.Api.Client;
using Pulse.Api.Client.Common;
using Pulse.Api.Client.Services;
using Pulse.Api.Ctrl.Client.Services.Interfaces;
using Pulse.Api.Ctrl.Contract;
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

    public async Task<ApiDataResult<IdentityDto>> Add(AddWorkflowRequest request,
        CancellationToken cancellationToken = default)
    {
        const string url = $"{BasePath}/add";
        return await SendForDataAsync<IdentityDto>(HttpMethod.Post, url, request, cancellationToken);
    }

    public async Task<ApiResult> Remove(RemoveWorkflowRequest request, CancellationToken cancellationToken = default)
    {
        const string url = $"{BasePath}/remove";
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