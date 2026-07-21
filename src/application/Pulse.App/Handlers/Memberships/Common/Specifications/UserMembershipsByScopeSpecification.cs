using System;
using System.Linq.Expressions;
using Pulse.App.Common.Database.Specifications.Base;
using Pulse.Domain.Aggregates.Memberships;
using Pulse.Domain.Aggregates.Users;
using Pulse.Domain.Common.Models.Enums;

namespace Pulse.App.Handlers.Memberships.Common.Specifications;

public sealed class UserMembershipsByScopeSpecification(
    UserId userId, Scope scope, bool includeDeleted = false) : Specification<Membership>
{
    public override Expression<Func<Membership, bool>> ToExpression()
    {
        Expression<Func<Membership, bool>> expression = membership => 
            membership.Scope == scope && membership.UserId == userId;
        
        if (!includeDeleted)
        {
            expression = expression.AndAlso(entity => !entity.IsDeleted);
        }

        return expression;
    }
}