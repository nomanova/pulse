using System;
using Pulse.App.Common.Services.Interfaces;

namespace Pulse.Infra.Services;

public sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}