using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pulse.Infra.Database.Configurations.Base;
using Pulse.Domain.Aggregates.Applications;

namespace Pulse.Infra.Database.Configurations;

public sealed class ApplicationConfiguration : DomainEntityTypeConfiguration<Application>
{
    public ApplicationConfiguration(DatabaseProvider provider) : base(provider)
    {
    }

    public override void Configure(EntityTypeBuilder<Application> builder)
    {
    }
}