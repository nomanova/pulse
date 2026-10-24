using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pulse.Domain.Aggregates.Applications;
using Pulse.Domain.Aggregates.Organizations;
using Pulse.Infra.Database.Configurations.Base;

namespace Pulse.Infra.Database.Configurations;

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

        builder.OwnsOne(application => application.Name, applicationBuilder =>
        {
            applicationBuilder
                .Property(name => name.Value)
                .IsRequired()
                .HasColumnName("name");

            applicationBuilder
                .Property(name => name.NormalizedValue)
                .IsRequired()
                .HasColumnName("normalized_name");
        });
        
        builder.OwnsOne(application => application.Key, applicationBuilder =>
        {
            applicationBuilder
                .Property(key => key.Value)
                .IsRequired()
                .HasColumnName("key");

            applicationBuilder
                .Property(key => key.NormalizedValue)
                .IsRequired()
                .HasColumnName("normalized_key");
        });

        // As long as https://github.com/dotnet/efcore/issues/38276 is not implemented,
        // the code below does not work (nested property in owned entity cannot be accessed).

        // builder.HasIndex(application => new
        //     {
        //         application.OrganizationId,
        //         application.Key.Value
        //     })
        //     .IsUnique();

        // Best alternative at the moment is to define the index manually in the migration.

        // Up
        // migrationBuilder.CreateIndex(
        //     name: "ix_applications_organization_id_key",
        //     table: "applications",
        //     columns: new[] { "organization_id", "key" },
        //     unique: true);

        // Down
        // migrationBuilder.DropIndex(
        //     name: "ix_applications_organization_id_key",
        //     table: "applications");
    }
}