using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pulse.Database.Configurations.Base;
using Pulse.Domain.Aggregates.Applications;
using Pulse.Domain.Aggregates.Organizations;

namespace Pulse.Database.Configurations;

public sealed class ApplicationConfiguration : DomainEntityTypeConfiguration<Application>
{
    public ApplicationConfiguration(DatabaseProvider provider) : base(provider)
    {
    }

    public override void Configure(EntityTypeBuilder<Application> builder)
    {
        base.Configure(builder);
        
        builder.HasKey(application => application.Id);
        
        builder.HasOne<Organization>()
            .WithMany()
            .HasForeignKey(application => application.OrganizationId)
            .IsRequired();
        
        builder.Property(application => application.Name)
            .IsRequired();

        builder.Property(application => application.NormalizedName)
            .IsRequired();
    }
}