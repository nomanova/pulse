namespace Pulse.Domain.Aggregates.Organizations;

public interface IOrganizationScoped<T>
{
    T OrganizationId { get; }
}