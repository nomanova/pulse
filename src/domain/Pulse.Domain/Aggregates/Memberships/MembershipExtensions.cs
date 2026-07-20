using System.Collections.Generic;
using System.Linq;
using Pulse.Domain.Aggregates.Organizations;
using Pulse.Domain.Aggregates.Roles;
using Pulse.Domain.Common.Models.Enums;

namespace Pulse.Domain.Aggregates.Memberships;

public static class MembershipExtensions
{
    extension(IEnumerable<Membership> memberships)
    {
        public bool IsSrvOwner()
        {
            return memberships.Any(membership => membership.Scope == Scope.Server &&
                                                 membership.RoleId == Role.BuiltIn.SrvOwner.Id);
        }

        public bool IsOrgOwner(OrganizationId organizationId)
        {
            return memberships.Any(membership => membership.OrganizationId == organizationId &&
                                                 membership.Scope == Scope.Organization &&
                                                 membership.RoleId == Role.BuiltIn.OrgOwner.Id);
        }
    }
}