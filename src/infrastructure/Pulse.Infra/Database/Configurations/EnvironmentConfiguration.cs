using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pulse.Domain.Aggregates.Applications;
using Pulse.Domain.Aggregates.Environments;
using Pulse.Domain.Aggregates.Organizations;
using Pulse.Infra.Database.Configurations.Base;

namespace Pulse.Infra.Database.Configurations;

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

        builder.OwnsOne(environment => environment.Name, environmentBuilder =>
        {
            environmentBuilder
                .Property(name => name.Value)
                .IsRequired()
                .HasColumnName("name");

            environmentBuilder
                .Property(name => name.NormalizedValue)
                .IsRequired()
                .HasColumnName("normalized_name");
        });

        // As long as https://github.com/dotnet/efcore/issues/38276 is not implemented,
        // the code below does not work (nested property in owned entity cannot be accessed).

        // builder.HasIndex(environment => new
        //     {
        //         environment.OrganizationId,
        //         environment.ApplicationId,
        //         environment.Name.Value
        //     })
        //     .IsUnique();

        // Best alternative at the moment is to define the index manually in the migration.

        // Up
        // migrationBuilder.CreateIndex(
        //     name: "ix_environments_organization_id_application_id_name",
        //     table: "environments",
        //     columns: new[] { "organization_id", "application_id", "name" },
        //     unique: true);

        // Down
        // migrationBuilder.DropIndex(
        //     name: "ix_environments_organization_id_application_id_name",
        //     table: "environments");
    }
}