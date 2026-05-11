using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pulse.Domain.Common.Models.Entities;

namespace Pulse.Infra.Database.Configurations.Base;

public abstract class DomainEntityTypeConfiguration<T> : EntityTypeConfiguration<T> where T : class, IDomainEntity
{
    protected DomainEntityTypeConfiguration(DatabaseProvider provider) : base(provider)
    {
    }

    public override void Configure(EntityTypeBuilder<T> builder)
    {
        builder.Ignore(entity => entity.Events);
        
        switch (Provider)
        {
            case DatabaseProvider.Sqlite:
                builder.Property(entity => entity.Version)
                    .IsConcurrencyToken()
                    .HasDefaultValue(0u);
                break;
            case DatabaseProvider.Postgres:
                builder.Property(entity => entity.Version)
                    .IsRowVersion();
                break;
            default:
                throw new NotImplementedException();
        }
    }
}