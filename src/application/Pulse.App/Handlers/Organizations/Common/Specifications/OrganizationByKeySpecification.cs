using Pulse.App.Common.Database.Specifications;
using Pulse.Domain.Aggregates.Organizations;

namespace Pulse.App.Handlers.Organizations.Common.Specifications;

public class OrganizationByKeySpecification(
    string? key,
    bool includeDeleted = false)
    : ByObjectKeySpecification<Organization, OrganizationId>(key, includeDeleted);