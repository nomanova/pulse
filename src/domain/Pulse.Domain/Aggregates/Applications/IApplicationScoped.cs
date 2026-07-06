using Pulse.Domain.Aggregates.Organizations;

namespace Pulse.Domain.Aggregates.Applications;

public interface IApplicationScoped : IOrganizationScoped
{
    ApplicationId ApplicationId { get; }
}