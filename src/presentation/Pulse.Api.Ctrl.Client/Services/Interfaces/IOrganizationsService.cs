using Pulse.Api.Client.Common;
using Pulse.Api.Ctrl.Contract.Organizations;
using Pulse.App.Dto.Common;

namespace Pulse.Api.Ctrl.Client.Services.Interfaces;

public interface IOrganizationsService
{
    Task<ApiDataResult<IdentityDto>> Create(CreateOrganizationRequest request,
        CancellationToken cancellationToken = default);
}