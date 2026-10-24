using Pulse.Domain.Common.Models.ValueObjects;

namespace Pulse.Domain.Common.Models.Entities;

public interface IKeyedObject
{
    ObjectKey Key { get; }
}