using System;
using System.Linq.Expressions;
using Pulse.App.Common.Database.Specifications.Base;
using Pulse.Domain.Common.Models.Entities;

namespace Pulse.App.Common.Database.Specifications;

public abstract class ByObjectKeySpecification<TEntity, Tk>(
    string? key,
    bool includeDeleted = false)
    : Specification<TEntity> where TEntity : DomainEntity<Tk>, IKeyedObject where Tk : EntityId
{
    public override Expression<Func<TEntity, bool>> ToExpression()
    {
        Expression<Func<TEntity, bool>> expression = entity => entity.Key.Value == key;

        if (!includeDeleted)
        {
            expression = expression.AndAlso(entity => !entity.IsDeleted);
        }

        return expression;
    }
}