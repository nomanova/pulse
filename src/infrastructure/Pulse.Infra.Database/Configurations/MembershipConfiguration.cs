using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pulse.Domain.Aggregates.Memberships;
using Pulse.Infra.Database.Configurations.Base;

namespace Pulse.Infra.Database.Configurations;

public sealed class MembershipConfiguration : DomainEntityTypeConfiguration<Membership>
{
    public MembershipConfiguration(DatabaseProvider provider) : base(provider)
    {
    }

    public override void Configure(EntityTypeBuilder<Membership> builder)
    {
        base.Configure(builder);
        
        // TODO
    }
}