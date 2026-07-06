using Pulse.Domain.Aggregates.Applications;

namespace Pulse.Domain.Aggregates.Environments;

public interface IEnvironmentScoped : IApplicationScoped
{
    EnvironmentId EnvironmentId { get; }
}