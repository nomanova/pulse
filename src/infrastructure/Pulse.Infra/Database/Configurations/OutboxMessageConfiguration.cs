using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pulse.Infra.Database.Configurations.Base;
using Pulse.Infra.Database.Messaging.Outbox;

namespace Pulse.Infra.Database.Configurations;

public sealed class OutboxMessageConfiguration : EntityTypeConfiguration<OutboxMessage>
{
    public OutboxMessageConfiguration(DatabaseProvider provider) : base(provider)
    {
    }

    public override void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.HasKey(message => message.Id);
        
        if (Provider == DatabaseProvider.Postgres)
        {
            // Use a partial index on OccurredOn, as processed messages
            // are no longer relevant to the processor.
            builder.HasIndex(message => message.OccurredOn)
                .HasFilter("processed_on IS NULL");

            // Use a partial index on ProcessedOn, as processed messages
            // are the only ones relevant to the archiver.
            builder.HasIndex(message => message.ProcessedOn)
                .HasFilter("processed_on IS NOT NULL");
            
            builder.Property(message => message.Content)
                .HasColumnType("jsonb");
        }
        else
        {
            builder.HasIndex(message => new
            {
                message.ProcessedOn,
                message.OccurredOn
            });
        }
    }
}