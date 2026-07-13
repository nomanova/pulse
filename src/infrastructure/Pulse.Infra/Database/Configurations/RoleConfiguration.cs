using Microsoft.EntityFrameworkCore;
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

        builder.HasKey(role => role.Id);

        builder.Property(role => role.Scope)
            .IsRequired()
            .HasConversion<string>();

        builder.OwnsOne(role => role.Name, roleBuilder =>
        {
            roleBuilder
                .Property(name => name.Value)
                .IsRequired()
                .HasColumnName("name");

            roleBuilder
                .Property(name => name.NormalizedValue)
                .IsRequired()
                .HasColumnName("normalized_name");
            
            roleBuilder.HasIndex(name => name.Value)
                .IsUnique();
        });
    }
}