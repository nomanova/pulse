using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pulse.Domain.Aggregates.Applications;
using Pulse.Domain.Aggregates.Environments;
using Pulse.Domain.Aggregates.Memberships;
using Pulse.Domain.Aggregates.Organizations;
using Pulse.Domain.Aggregates.Roles;
using Pulse.Domain.Aggregates.Users;
using Pulse.Infra.Database.Configurations.Base;

namespace Pulse.Infra.Database.Configurations;

public sealed class MembershipConfiguration : DomainEntityTypeConfiguration<Membership>
{
    public MembershipConfiguration(DatabaseProvider provider) : base(provider)
    {
    }

    public override void Configure(EntityTypeBuilder<Membership> builder)
    {
        base.Configure(builder);

        builder.HasKey(membership => membership.Id);

        builder.Property(membership => membership.Scope)
            .IsRequired()
            .HasConversion<string>();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(membership => membership.UserId)
            .IsRequired();

        builder.HasOne<Role>()
            .WithMany()
            .HasForeignKey(membership => membership.RoleId)
            .IsRequired();

        builder.HasOne<Organization>()
            .WithMany()
            .HasForeignKey(membership => membership.OrganizationId);

        builder.HasOne<Application>()
            .WithMany()
            .HasForeignKey(membership => membership.ApplicationId);

        builder.HasOne<Environment>()
            .WithMany()
            .HasForeignKey(membership => membership.EnvironmentId);

        // As a user should be a member of a particular resource at most once,
        // which is enforced on DB level.
        builder
            .HasIndex(membership => new
            {
                membership.UserId,
                membership.OrganizationId,
                membership.ApplicationId,
                membership.EnvironmentId
            })
            .IsUnique();
    }
}