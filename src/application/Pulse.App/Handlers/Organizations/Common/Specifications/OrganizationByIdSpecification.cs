using Pulse.App.Common.Database.Specifications;
using Pulse.Domain.Aggregates.Organizations;

namespace Pulse.App.Handlers.Organizations.Common.Specifications;

public sealed class OrganizationByIdSpecification(
    OrganizationId id,
    bool includeDeleted = false) : ByIdSpecification<Organization, OrganizationId>(id, includeDeleted);
    