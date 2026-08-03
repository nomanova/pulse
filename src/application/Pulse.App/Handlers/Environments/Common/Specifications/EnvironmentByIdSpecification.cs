using Pulse.App.Common.Database.Specifications;
using Pulse.Domain.Aggregates.Environments;

namespace Pulse.App.Handlers.Environments.Common.Specifications;

public sealed class EnvironmentByIdSpecification(EnvironmentId id, bool includeDeleted = false) :
    ByIdSpecification<Environment, EnvironmentId>(id, includeDeleted);