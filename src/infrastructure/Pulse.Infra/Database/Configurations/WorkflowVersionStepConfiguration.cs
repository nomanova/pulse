using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pulse.Domain.Aggregates.Workflows.Entities;
using Pulse.Domain.Aggregates.Workflows.ValueObjects;
using Pulse.Infra.Database.Configurations.Base;
using Pulse.Infra.Database.Converters;

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

        builder.Property(step => step.Definition)
            .IsRequired()
            .HasConversion<JsonValueConverter<IWorkflowStepDefinition>>();
        
        if (Provider == DatabaseProvider.Postgres)
        {
            builder.Property(step => step.Definition)
                .HasColumnType("jsonb");
        }
        
        builder.HasIndex(step => new
        {
            step.WorkflowVersionId,
            step.Order
        }).IsUnique();
    }
}