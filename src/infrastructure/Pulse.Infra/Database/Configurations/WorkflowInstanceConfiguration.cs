using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pulse.Domain.Aggregates.Applications;
using Pulse.Domain.Aggregates.Environments;
using Pulse.Domain.Aggregates.Organizations;
using Pulse.Domain.Aggregates.WorkflowInstances;
using Pulse.Domain.Aggregates.Workflows;
using Pulse.Domain.Aggregates.Workflows.Entities;
using Pulse.Infra.Database.Configurations.Base;

namespace Pulse.Infra.Database.Configurations;

public sealed class WorkflowInstanceConfiguration : DomainEntityTypeConfiguration<WorkflowInstance>
{
    public WorkflowInstanceConfiguration(DatabaseProvider provider) : base(provider)
    {
    }

    public override void Configure(EntityTypeBuilder<WorkflowInstance> builder)
    {
        base.Configure(builder);

        builder.HasKey(instance => instance.Id);

        builder.Property(instance => instance.Source)
            .IsRequired()
            .HasConversion<string>();
        
        builder.HasOne<Environment>()
            .WithMany()
            .HasForeignKey(instance => instance.EnvironmentId)
            .IsRequired();
        
        builder.HasOne<WorkflowVersion>()
            .WithMany()
            .HasForeignKey(instance => instance.WorkflowVersionId);

        builder.Property(instance => instance.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.HasMany(instance => instance.Steps)
            .WithOne()
            .HasForeignKey(step => step.WorkflowInstanceId)
            .IsRequired()
            .HasPrincipalKey(instance => instance.Id)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(instance => instance.Steps).AutoInclude();
    }
}