using System;

namespace Pulse.App.Common.Services.Interfaces;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}