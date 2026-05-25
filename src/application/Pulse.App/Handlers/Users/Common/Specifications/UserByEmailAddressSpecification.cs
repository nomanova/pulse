using System;
using System.Linq.Expressions;
using Pulse.App.Common.Database.Specifications.Base;
using Pulse.Domain.Aggregates.Users;
using Pulse.Domain.Aggregates.Users.ValueObjects;

namespace Pulse.App.Handlers.Users.Common.Specifications;

public sealed class UserByEmailAddressSpecification : Specification<User>
{
    private readonly string? _emailAddress;

    public static UserByEmailAddressSpecification New(string emailAddress)
    {
        return new UserByEmailAddressSpecification(emailAddress);
    }

    private UserByEmailAddressSpecification(string emailAddress)
    {
        var emailAddressResult = EmailAddress.Create(emailAddress);
        _emailAddress = emailAddressResult.IsError ? null : emailAddressResult.Value.NormalizedValue;
    }

    public override Expression<Func<User, bool>> ToExpression()
    {
        if (_emailAddress is null)
        {
            return user => false;
        }

        return user => user.EmailAddress != null &&
                       user.EmailAddress.NormalizedValue == _emailAddress &&
                       !user.IsDeleted;
    }
}