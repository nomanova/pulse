using System.Collections.Generic;
using System.Linq;
using Pulse.Domain.Aggregates.Organizations;
using Pulse.Domain.Aggregates.Roles;
using Pulse.Domain.Common.Models.Enums;

namespace Pulse.Domain.Aggregates.Memberships;

public static class MembershipExtensions
{
    public static bool IsOrgOwner(this IEnumerable<Membership> memberships, OrganizationId organizationId)
    {
        return memberships.Any(m => m.OrganizationId == organizationId &&
                                    m.Scope == Scope.Organization &&
                                    m.RoleId == Role.BuiltIn.OrgOwner.Id);
    }
}