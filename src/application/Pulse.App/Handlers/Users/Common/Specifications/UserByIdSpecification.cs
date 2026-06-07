using Pulse.App.Common.Database.Specifications;
using Pulse.Domain.Aggregates.Users;

namespace Pulse.App.Handlers.Users.Common.Specifications;

public sealed class UserByIdSpecification(
    UserId id,
    bool includeDeleted = false) : ByIdSpecification<User, UserId>(id, includeDeleted);