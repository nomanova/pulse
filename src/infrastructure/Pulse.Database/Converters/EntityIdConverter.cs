using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pulse.Domain.Common.Models.Entities;

namespace Pulse.Database.Converters;

public class EntityIdConverter<TId> : ValueConverter<TId, string> 
    where TId : EntityId, INew<TId>
{
    public EntityIdConverter()
        : base(
            id => id.Value,
            value => Create(value))
    {
    }

    private static TId Create(string value) => TId.New(value);
}
