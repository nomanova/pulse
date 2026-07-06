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

        builder.Property(workflow => workflow.Name)
            .IsRequired();

        builder.Property(workflow => workflow.NormalizedName)
            .IsRequired();

        builder.HasMany(workflow => workflow.Steps)
            .WithOne()
            .HasForeignKey(workflowStep => workflowStep.WorkflowId)
            .IsRequired()
            .HasPrincipalKey(workflow => workflow.Id)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Navigation(workflow => workflow.Steps).AutoInclude();
    }
}