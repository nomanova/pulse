using Pulse.Domain.Aggregates.Applications;
using Pulse.Domain.Aggregates.Environments;
using Pulse.Domain.Aggregates.Organizations;
using Pulse.Domain.Common.Models.Enums;

namespace Pulse.App.Common.Authorization;

public sealed record ResourceAuthorizationContext
{
    public required Scope Scope { get; init; }
    
    public required OrganizationId OrganizationId { get; init; }
    
    public ApplicationId? ApplicationId { get; init; }
    
    public EnvironmentId? EnvironmentId { get; init; }
}