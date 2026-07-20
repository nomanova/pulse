using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Pulse.Domain.Common.Models.Entities;
using Pulse.Domain.Common.Models.ValueObjects;
using Throw;

namespace Pulse.App.Common.Database.Specifications;

public sealed class OrderBySpecification<T, Tk, Tp> : IOrderBySpecification<T>
    where T : Entity<Tk> where Tk : EntityId where Tp : IComparable
{
    private readonly string _property;
    private readonly bool _ascending;

    private readonly ParameterExpression _paramExpr;
    private readonly MemberExpression _propExpr;
    private readonly Expression<Func<T, Tp>> _orderBy;

    public OrderBySpecification(
        string property,
        bool? ascending = null)
    {
        _property = property;
        _ascending = ascending ?? true;

        _paramExpr = Expression.Parameter(typeof(T), "item");
        _propExpr = CreateComparablePropertyExpression(_paramExpr, property);

        _orderBy = Expression.Lambda<Func<T, Tp>>(_propExpr, _paramExpr);
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

        var rawValue = propertyInfo.GetValue(entity, null);
        var value = rawValue is ObjectName objectName
            ? (Tp?)(object)objectName.Value
            : (Tp?)rawValue;

        Expression<Func<Tp?>> propLambda = () => value;
        return Comparable(propLambda);
    }

    public Expression<Func<T, bool>> Comparable(List<T> entities)
    {
        Expression<Func<Tp?>> propLambda = () => _ascending ? MaxValue(entities) : MinValue(entities);
        return Comparable(propLambda);
    }

    private Expression<Func<T, bool>> Comparable(Expression<Func<Tp?>> propLambda)
    {
        var callMethod = typeof(Tp).GetMethod("CompareTo", [typeof(Tp)]);
        callMethod.ThrowIfNull();

        var callExpr = Expression.Call(_propExpr, callMethod, propLambda.Body);

        var searchExpr = _ascending
            ? Expression.GreaterThan(callExpr, Expression.Constant(0))
            : Expression.LessThan(callExpr, Expression.Constant(0));

        return Expression.Lambda<Func<T, bool>>(searchExpr, _paramExpr);
    }

    private static MemberExpression CreateComparablePropertyExpression(ParameterExpression paramExpr, string property)
    {
        var propExpr = Expression.Property(paramExpr, property);

        if (propExpr.Type == typeof(Tp))
        {
            return propExpr;
        }

        if (propExpr.Type == typeof(ObjectName))
        {
            var valueExpr = Expression.Property(propExpr, nameof(ObjectName.Value));

            if (valueExpr.Type == typeof(Tp))
            {
                return valueExpr;
            }
        }

        throw new InvalidOperationException(
            $"Property '{property}' on '{typeof(T).Name}' cannot be used for ordering as '{typeof(Tp).Name}'.");
    }

    private Tp? MaxValue(List<T> entities)
    {
        return entities.Max(_orderBy.Compile());
    }

    private Tp? MinValue(List<T> entities)
    {
        return entities.Min(_orderBy.Compile());
    }
}