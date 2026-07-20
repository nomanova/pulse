using Pulse.Api.Shared.Contract;

namespace Pulse.Api.Ctrl.Contract.Applications;

public sealed record SearchApplicationsRequest : PagedSearchRequest
{
    public string? OrganizationName { get; set; }
}