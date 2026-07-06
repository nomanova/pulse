using Pulse.App.Common.Database;
using Pulse.Domain.Aggregates.Environments;

namespace Pulse.App.Handlers.Environments.Common;

public interface IEnvironmentRepository : IRepository<Environment>;

internal sealed class EnvironmentRepository : Repository<Environment>, IEnvironmentRepository
{
    public EnvironmentRepository(IDatabaseContext context) : base(context.Environments)
    {
    }
}