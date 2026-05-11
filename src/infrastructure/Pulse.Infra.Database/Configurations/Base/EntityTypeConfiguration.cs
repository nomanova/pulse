using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pulse.Infra.Database.Configurations.Base;

public abstract class EntityTypeConfiguration<T> : IEntityTypeConfiguration<T> where T : class
{
    protected readonly DatabaseProvider Provider;
    
    protected EntityTypeConfiguration(DatabaseProvider provider)
    {
        Provider = provider;
    }

    public abstract void Configure(EntityTypeBuilder<T> builder);
}