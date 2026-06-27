using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pulse.Database.Configurations.Base;
using Pulse.Domain.Aggregates.Roles;

namespace Pulse.Database.Configurations;

public sealed class RoleConfiguration : DomainEntityTypeConfiguration<Role>
{
    public RoleConfiguration(DatabaseProvider provider) : base(provider)
    {
    }

    public override void Configure(EntityTypeBuilder<Role> builder)
    {
        base.Configure(builder);

        builder.HasKey(role => role.Id);

        builder.Property(role => role.Scope)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(role => role.Name)
            .IsRequired();

        builder.Property(role => role.NormalizedName)
            .IsRequired();
    }
}