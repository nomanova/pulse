using System;

namespace Pulse.Domain.Common.Models.Entities;

public interface ITimestamped
{
    DateTime CreatedAt { get; }

    DateTime? ModifiedAt { get; }
}