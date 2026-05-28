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

        builder.Property(role => role.Source)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(role => role.Scope)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(role => role.Name)
            .IsRequired();

        builder.Property(role => role.NormalizedName)
            .IsRequired();

        builder.HasMany(role => role.Permission)
            .WithOne()
            .HasForeignKey(permission => permission.RoleId)
            .IsRequired()
            .HasPrincipalKey(role => role.Id)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(role => role.Permission).AutoInclude();
    }
}