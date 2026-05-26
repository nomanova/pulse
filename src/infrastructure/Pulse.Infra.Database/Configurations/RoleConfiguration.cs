using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pulse.Domain.Aggregates.Roles;
using Pulse.Infra.Database.Configurations.Base;

namespace Pulse.Infra.Database.Configurations;

public sealed class RoleConfiguration : DomainEntityTypeConfiguration<Role>
{
    public RoleConfiguration(DatabaseProvider provider) : base(provider)
    {
    }

    public override void Configure(EntityTypeBuilder<Role> builder)
    {
        base.Configure(builder);
        
        // TODO
    }
}