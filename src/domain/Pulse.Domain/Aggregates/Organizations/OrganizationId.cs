using Pulse.Domain.Common.Models.Entities;

namespace Pulse.Domain.Aggregates.Organizations;

public sealed record OrganizationId : EntityId, INew<OrganizationId>
{
    public static OrganizationId New(string value)
    {
        return new OrganizationId { Value = value };
    }
}