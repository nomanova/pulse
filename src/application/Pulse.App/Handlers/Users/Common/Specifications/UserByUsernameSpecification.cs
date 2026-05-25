using System;
using System.Linq.Expressions;
using Pulse.App.Common.Database.Specifications.Base;
using Pulse.Domain.Aggregates.Users;
using Pulse.Domain.Aggregates.Users.ValueObjects;

namespace Pulse.App.Handlers.Users.Common.Specifications;

public class UserByUsernameSpecification : Specification<User>
{
    private readonly string? _username;
    
    public static UserByUsernameSpecification New(string? username)
    {
        return new UserByUsernameSpecification(username);
    }

    private UserByUsernameSpecification(string? username)
    {
        var usernameResult = Username.Create(username);
        _username = usernameResult.IsError ? null : usernameResult.Value.Value;
    }
    
    public override Expression<Func<User, bool>> ToExpression()
    {
        if (_username is null)
        {
            return user => false;
        }

        return user => user.Username.Value == _username &&
                       !user.IsDeleted;
    }
}