using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pulse.Domain.Aggregates.Workflows.Entities;
using Pulse.Infra.Database.Configurations.Base;

namespace Pulse.Infra.Database.Configurations;

public sealed class WorkflowVersionConfiguration : EntityTypeConfiguration<WorkflowVersion>
{
    public WorkflowVersionConfiguration(DatabaseProvider provider) : base(provider)
    {
    }

    public override void Configure(EntityTypeBuilder<WorkflowVersion> builder)
    {
        builder.HasKey(version => version.Id);

        builder.Property(version => version.Version)
            .IsRequired();

        builder.Property(version => version.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.HasMany(version => version.Steps)
            .WithOne()
            .HasForeignKey(step => step.WorkflowVersionId)
            .IsRequired()
            .HasPrincipalKey(version => version.Id)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(version => version.Steps).AutoInclude();

        builder.HasIndex(version => new
        {
            version.WorkflowId,
            version.Version
        }).IsUnique();
    }
}