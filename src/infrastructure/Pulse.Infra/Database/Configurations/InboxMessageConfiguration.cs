using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pulse.Infra.Database.Configurations.Base;
using Pulse.Infra.Database.Messaging.Inbox;

namespace Pulse.Infra.Database.Configurations;

public sealed class InboxMessageConfiguration : EntityTypeConfiguration<InboxMessage>
{
    public InboxMessageConfiguration(DatabaseProvider provider) : base(provider)
    {
    }

    public override void Configure(EntityTypeBuilder<InboxMessage> builder)
    {
        builder.HasKey(message => new
        {
            message.Id,
            message.Handler
        });

        builder.HasIndex(message => message.ProcessedOn);
    }
}