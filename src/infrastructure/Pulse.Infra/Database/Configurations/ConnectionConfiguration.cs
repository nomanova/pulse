using System.Collections.Generic;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pulse.Domain.Aggregates.Connections;
using Pulse.Infra.Database.Configurations.Base;
using Pulse.Infra.Database.Converters;
using Pulse.Infra.Database.ValueComparers;
using Environment = Pulse.Domain.Aggregates.Environments.Environment;

namespace Pulse.Infra.Database.Configurations;

public sealed class ConnectionConfiguration : DomainEntityTypeConfiguration<Connection>
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);

    public ConnectionConfiguration(DatabaseProvider provider) : base(provider)
    {
    }

    public override void Configure(EntityTypeBuilder<Connection> builder)
    {
        base.Configure(builder);

        builder.HasKey(connection => connection.Id);

        builder.HasOne<Environment>()
            .WithMany()
            .HasForeignKey(connection => connection.EnvironmentId)
            .IsRequired();

        builder.Property(connection => connection.Channel)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(connection => connection.PluginId)
            .IsRequired();
        
        builder.Property(connection => connection.Parameters)
            .IsRequired()
            .HasConversion<JsonValueConverter<Dictionary<string, string>>>()
            .Metadata.SetValueComparer(new DictionaryValueComparer<string, string>());

        if (Provider == DatabaseProvider.Postgres)
        {
            builder.Property(connection => connection.Parameters)
                .HasColumnType("jsonb");
        }
        
        builder.HasIndex(connection => new
            {
                connection.EnvironmentId,
                connection.PluginId
            })
            .IsUnique();
    }
}