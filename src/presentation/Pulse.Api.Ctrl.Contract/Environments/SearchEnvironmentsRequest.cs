using Pulse.Api.Shared.Contract;

namespace Pulse.Api.Ctrl.Contract.Environments;

public sealed record SearchEnvironmentsRequest : PagedSearchRequest
{
    public string? OrganizationName { get; init; }
    
    public string? ApplicationName { get; init; }
}