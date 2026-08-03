using Pulse.Domain.Aggregates.Applications;

namespace Pulse.Domain.Aggregates.Environments;

public interface IEnvironmentScoped
{
    EnvironmentId EnvironmentId { get; }
}