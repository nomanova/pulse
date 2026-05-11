using System;

namespace Pulse.Domain.Common.Models.Entities;

/// <summary>
/// Marker interface for (soft) deltetable entities
/// </summary>
public interface IDeletable
{
    DateTime? DeletedAt { get; }

    bool IsDeleted { get; }

    void SetDeleted();
}