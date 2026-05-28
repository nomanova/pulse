using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Pulse.App.Common.Database;
using Pulse.Domain.Aggregates.Memberships;

namespace Pulse.App.Handlers.Memberships.Common;

public interface IMembershipRepository : IRepository<Membership>;

internal sealed class MembershipRepository : Repository<Membership>, IMembershipRepository
{
    public MembershipRepository(IDatabaseContext context) : base(context.Memberships)
    {
    }
}