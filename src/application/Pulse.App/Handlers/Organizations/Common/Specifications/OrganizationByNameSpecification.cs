using Pulse.App.Common.Database.Specifications;
using Pulse.Domain.Aggregates.Organizations;

namespace Pulse.App.Handlers.Organizations.Common.Specifications;

public class OrganizationByNameSpecification(
    string name,
    bool includeDeleted = false)
    : ByNameSpecification<Organization, OrganizationId>(name, includeDeleted);