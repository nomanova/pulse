using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pulse.Domain.Aggregates.Workflows.Entities;
using Pulse.Infra.Database.Configurations.Base;

namespace Pulse.Infra.Database.Configurations;

public sealed class WorkflowStepConfiguration : EntityTypeConfiguration<WorkflowStep>
{
    public WorkflowStepConfiguration(DatabaseProvider provider) : base(provider)
    {
    }

    public override void Configure(EntityTypeBuilder<WorkflowStep> builder)
    {
        builder.HasKey(step => step.Id);
    }
}