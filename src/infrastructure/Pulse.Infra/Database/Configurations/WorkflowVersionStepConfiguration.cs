using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pulse.Domain.Aggregates.Workflows.Entities;
using Pulse.Infra.Database.Configurations.Base;

namespace Pulse.Infra.Database.Configurations;

public sealed class WorkflowVersionStepConfiguration : EntityTypeConfiguration<WorkflowVersionStep>
{
    public WorkflowVersionStepConfiguration(DatabaseProvider provider) : base(provider)
    {
    }

    public override void Configure(EntityTypeBuilder<WorkflowVersionStep> builder)
    {
        builder.HasKey(step => step.Id);

        builder.Property(step => step.Order)
            .IsRequired();

        builder.HasIndex(step => new
        {
            step.WorkflowVersionId,
            step.Order
        }).IsUnique();
    }
}