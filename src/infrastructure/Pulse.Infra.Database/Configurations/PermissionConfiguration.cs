using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pulse.Domain.Aggregates.Roles.Entities;

namespace Pulse.Infra.Database.Configurations;

public sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.HasKey(permission => permission.Id);
        
        builder.Property(permission => permission.Key)
            .IsRequired();
    }
}