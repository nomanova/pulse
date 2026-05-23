using Pulse.Domain.Common.Models.Entities;

namespace Pulse.Domain.Aggregates.Users;

public sealed record UserId : EntityId, INew<UserId>
{
    public static UserId New(string value)
    {
        return new UserId { Value = value };
    }
}