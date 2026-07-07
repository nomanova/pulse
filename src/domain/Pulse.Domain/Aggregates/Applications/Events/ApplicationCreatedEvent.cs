using Pulse.Domain.Aggregates.Organizations;
using Pulse.Domain.Common.Models.Events;

namespace Pulse.Domain.Aggregates.Applications.Events;

public sealed class ApplicationCreatedEvent : IDomainEvent
{
    public OrganizationId OrganizationId { get; }

    public ApplicationId ApplicationId { get; }

    public ApplicationCreatedEvent(OrganizationId organizationId, ApplicationId applicationId)
    {
        OrganizationId = organizationId;
        ApplicationId = applicationId;
    }
}