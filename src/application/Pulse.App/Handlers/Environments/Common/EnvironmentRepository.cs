using Pulse.App.Common.Database;
using Pulse.Domain.Aggregates.Environments;

namespace Pulse.App.Handlers.Environments.Common;

public interface IEnvironmentRepository : IRepository<Environment>;

public class EnvironmentRepository : Repository<Environment>, IEnvironmentRepository
{
    protected EnvironmentRepository(IDatabaseContext context) : base(context.Environments)
    {
    }
}