using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pulse.Domain.Aggregates.WorkflowInstances.Entities;
using Pulse.Domain.Aggregates.Workflows.Entities;
using Pulse.Infra.Database.Configurations.Base;

namespace Pulse.Infra.Database.Configurations;

public sealed class WorkflowInstanceStepConfiguration : EntityTypeConfiguration<WorkflowInstanceStep>
{
    public WorkflowInstanceStepConfiguration(DatabaseProvider provider) : base(provider)
    {
    }

    public override void Configure(EntityTypeBuilder<WorkflowInstanceStep> builder)
    {
        builder.HasKey(step => step.Id);

        builder.HasOne<WorkflowVersionStep>()
            .WithMany()
            .HasForeignKey(step => step.WorkflowVersionStepId)
            .IsRequired();

        builder.Property(step => step.Order)
            .IsRequired();

        builder.Property(step => step.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.HasIndex(step => new
        {
            step.WorkflowInstanceId,
            step.Order
        }).IsUnique();

        builder.HasIndex(step => step.WorkflowVersionStepId);
    }
}