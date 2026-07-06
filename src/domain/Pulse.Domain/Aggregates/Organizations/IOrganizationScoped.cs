namespace Pulse.Domain.Aggregates.Organizations;

public interface IOrganizationScoped
{
    OrganizationId OrganizationId { get; }
}