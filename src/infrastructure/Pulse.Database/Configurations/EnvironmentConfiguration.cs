using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pulse.Database.Configurations.Base;
using Pulse.Domain.Aggregates.Applications;
using Pulse.Domain.Aggregates.Environments;
using Pulse.Domain.Aggregates.Organizations;

namespace Pulse.Database.Configurations;

public sealed class EnvironmentConfiguration : DomainEntityTypeConfiguration<Environment>
{
    public EnvironmentConfiguration(DatabaseProvider provider) : base(provider)
    {
    }

    public override void Configure(EntityTypeBuilder<Environment> builder)
    {
        base.Configure(builder);

        builder.HasKey(environment => environment.Id);
        
        builder.HasOne<Organization>()
            .WithMany()
            .HasForeignKey(environment => environment.OrganizationId)
            .IsRequired();
        
        builder.HasOne<Application>()
            .WithMany()
            .HasForeignKey(environment => environment.ApplicationId)
            .IsRequired();
        
        builder.Property(environment => environment.Name)
            .IsRequired();

        builder.Property(environment => environment.NormalizedName)
            .IsRequired();
    }
}