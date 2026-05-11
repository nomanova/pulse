using Pulse.Domain.Common.Models.Entities;

namespace Pulse.Domain.Aggregates.Applications;

public sealed record ApplicationId : EntityId, INew<ApplicationId>
{
    public static ApplicationId New(string value)
    {
        return new ApplicationId { Value = value };
    }
}