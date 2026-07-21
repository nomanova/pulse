using System;
using System.Linq.Expressions;
using Pulse.App.Common.Database.Specifications.Base;
using Pulse.Domain.Common.Models.Entities;

namespace Pulse.App.Common.Database.Specifications;

public abstract class ByObjectNameSpecification<TEntity, Tk>(
    string? name,
    bool includeDeleted = false)
    : Specification<TEntity> where TEntity : DomainEntity<Tk>, INamedObject where Tk : EntityId
{
    public override Expression<Func<TEntity, bool>> ToExpression()
    {
        Expression<Func<TEntity, bool>> expression = entity => entity.Name.Value == name;

        if (!includeDeleted)
        {
            expression = expression.AndAlso(entity => !entity.IsDeleted);
        }

        return expression;
    }
}