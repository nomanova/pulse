using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pulse.Domain.Aggregates.Applications;
using Pulse.Domain.Aggregates.Environments;
using Pulse.Domain.Aggregates.Organizations;
using Pulse.Domain.Aggregates.Workflows;
using Pulse.Infra.Database.Configurations.Base;

namespace Pulse.Infra.Database.Configurations;

public sealed class WorkflowConfiguration : DomainEntityTypeConfiguration<Workflow>
{
    public WorkflowConfiguration(DatabaseProvider provider) : base(provider)
    {
    }

    public override void Configure(EntityTypeBuilder<Workflow> builder)
    {
        base.Configure(builder);

        builder.HasKey(workflow => workflow.Id);

        builder.HasOne<Organization>()
            .WithMany()
            .HasForeignKey(workflow => workflow.OrganizationId)
            .IsRequired();

        builder.HasOne<Application>()
            .WithMany()
            .HasForeignKey(workflow => workflow.ApplicationId)
            .IsRequired();

        builder.HasOne<Environment>()
            .WithMany()
            .HasForeignKey(workflow => workflow.EnvironmentId)
            .IsRequired();

        builder.OwnsOne(workflow => workflow.Name, workflowBuilder =>
        {
            workflowBuilder
                .Property(name => name.Value)
                .IsRequired()
                .HasColumnName("name");

            workflowBuilder
                .Property(name => name.NormalizedValue)
                .IsRequired()
                .HasColumnName("normalized_name");
        });

        builder.HasMany(workflow => workflow.Versions)
            .WithOne()
            .HasForeignKey(version => version.WorkflowId)
            .IsRequired()
            .HasPrincipalKey(workflow => workflow.Id)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Navigation(workflow => workflow.Versions).AutoInclude();
        
        // As long as https://github.com/dotnet/efcore/issues/38276 is not implemented,
        // the code below does not work (nested property in owned entity cannot be accessed).

        // builder.HasIndex(workflow => new
        //     {
        //         workflow.OrganizationId,
        //         workflow.ApplicationId,
        //         workflow.EnvironmentId,
        //         workflow.Name.Value
        //     })
        //     .IsUnique();

        // Best alternative at the moment is to define the index manually in the migration.

        // Up
        // migrationBuilder.CreateIndex(
        //     name: "ix_workflows_organization_id_application_id_environment_id_name",
        //     table: "workflows",
        //     columns: new[] { "organization_id", "application_id", "environment_id", "name" },
        //     unique: true);

        // Down
        // migrationBuilder.DropIndex(
        //     name: "ix_workflows_organization_id_application_id_environment_id_name",
        //     table: "applications");
    }
}