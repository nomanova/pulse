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
        
        builder.Property(organization => organization.Name)
            .IsRequired();
        
        builder.Property(organization => organization.NormalizedName)
            .IsRequired();
    }
}