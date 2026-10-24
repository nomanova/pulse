using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pulse.Domain.Aggregates.Organizations;
using Pulse.Infra.Database.Configurations.Base;

namespace Pulse.Infra.Database.Configurations;

public sealed class OrganizationConfiguration : DomainEntityTypeConfiguration<Organization>
{
    public OrganizationConfiguration(DatabaseProvider provider) : base(provider)
    {
    }

    public override void Configure(EntityTypeBuilder<Organization> builder)
    {
        base.Configure(builder);

        builder.HasKey(user => user.Id);

        builder.OwnsOne(organization => organization.Name, organizationBuilder =>
        {
            organizationBuilder
                .Property(name => name.Value)
                .IsRequired()
                .HasColumnName("name");

            organizationBuilder
                .Property(name => name.NormalizedValue)
                .IsRequired()
                .HasColumnName("normalized_name");
        });

        builder.OwnsOne(organization => organization.Key, organizationBuilder =>
        {
            organizationBuilder
                .Property(key => key.Value)
                .IsRequired()
                .HasColumnName("key");

            organizationBuilder
                .Property(key => key.NormalizedValue)
                .IsRequired()
                .HasColumnName("normalized_key");
            
            organizationBuilder.HasIndex(key => key.Value)
                .IsUnique();
        });
    }
}