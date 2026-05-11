using Pulse.Domain.Common.Models.Events;

namespace Pulse.Domain.Common.Models.Entities;

public interface IDomainEntity : IDomainEventEmitter, ITimestamped, IDeletable, IVersioned;