using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pulse.Infra.Database.Configurations.Base;
using Pulse.Infra.Database.Messaging.Events;

namespace Pulse.Infra.Database.Configurations;

public sealed class EventConfiguration : EntityTypeConfiguration<Event>
{
    public EventConfiguration(DatabaseProvider provider) : base(provider)
    {
    }

    public override void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.HasKey(@event => @event.Id);
        
        if (Provider == DatabaseProvider.Postgres)
        {
            // On postgres, a partial index can be used as 
            // processed messages are no longer relevant to the processor.
            builder.HasIndex(message => message.OccurredOn)
                .HasFilter("processed_on IS NULL");

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