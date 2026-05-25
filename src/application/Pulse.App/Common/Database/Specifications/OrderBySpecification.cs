using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Pulse.Domain.Common.Models.Entities;
using Throw;

namespace Pulse.App.Common.Database.Specifications;

public sealed class OrderBySpecification<T, TK, TP> : IOrderBySpecification<T>
    where T : Entity<TK> where TK : EntityId where TP : IComparable
{
    private readonly string _property;
    private readonly bool _ascending;

    private readonly ParameterExpression _paramExpr;
    private readonly MemberExpression _propExpr;
    private readonly Expression<Func<T, TP>> _orderBy;

    public OrderBySpecification(
        string property,
        bool? ascending = null)
    {
        _property = property;
        _ascending = ascending ?? true;

        _paramExpr = Expression.Parameter(typeof(T), "item");
        _propExpr = Expression.Property(_paramExpr, property);

        _orderBy = Expression.Lambda<Func<T, TP>>(_propExpr, _paramExpr);
    }

    /**
     * As the property values are not guaranteed to be unique,
     * ensure deterministic ordering by including the id as well.
     */
    public IQueryable<T> ApplyOrdering(IQueryable<T> queryable)
    {
        return _ascending
            ? queryable.OrderBy(_orderBy).ThenBy(entity => entity.Id)
            : queryable.OrderByDescending(_orderBy).ThenByDescending(entity => entity.Id);
    }

    public Expression<Func<T, bool>> Comparable(T entity)
    {
        var propertyInfo = typeof(T).GetProperty(_property);
        propertyInfo.ThrowIfNull();
        
        var value = (TP?)propertyInfo.GetValue(entity, null);

        Expression<Func<TP?>> propLambda = () => value;
        return Comparable(propLambda);
    }

    public Expression<Func<T, bool>> Comparable(List<T> entities)
    {
        Expression<Func<TP?>> propLambda = () => _ascending ? MaxValue(entities) : MinValue(entities);
        return Comparable(propLambda);
    }

    private Expression<Func<T, bool>> Comparable(Expression<Func<TP?>> propLambda)
    {
        var callMethod = typeof(TP).GetMethod("CompareTo", [typeof(TP)]);
        callMethod.ThrowIfNull();

        var callExpr = Expression.Call(_propExpr, callMethod, propLambda.Body);

        var searchExpr = _ascending
            ? Expression.GreaterThan(callExpr, Expression.Constant(0))
            : Expression.LessThan(callExpr, Expression.Constant(0));

        return Expression.Lambda<Func<T, bool>>(searchExpr, _paramExpr);
    }

    private TP? MaxValue(List<T> entities)
    {
        return entities.Max(_orderBy.Compile());
    }

    private TP? MinValue(List<T> entities)
    {
        return entities.Min(_orderBy.Compile());
    }
}