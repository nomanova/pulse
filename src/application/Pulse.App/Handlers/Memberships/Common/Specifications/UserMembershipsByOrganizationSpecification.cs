using System;
using System.Linq.Expressions;
using Pulse.App.Common.Database.Specifications.Base;
using Pulse.Domain.Aggregates.Memberships;
using Pulse.Domain.Aggregates.Organizations;
using Pulse.Domain.Aggregates.Users;

namespace Pulse.App.Handlers.Memberships.Common.Specifications;

public sealed class UserMembershipsByOrganizationSpecification(
    UserId userId,
    OrganizationId organizationId,
    bool includeDeleted = false) : Specification<Membership>
{
    public override Expression<Func<Membership, bool>> ToExpression()
    {
        Expression<Func<Membership, bool>> expression = membership => 
            membership.OrganizationId == organizationId && membership.UserId == userId;
        
        if (!includeDeleted)
        {
            expression = expression.AndAlso(entity => !entity.IsDeleted);
        }

        return expression;
    }
}