using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pulse.Database.Configurations.Base;
using Pulse.Domain.Aggregates.Users;
using Pulse.Domain.Aggregates.Users.ValueObjects;

namespace Pulse.Database.Configurations;

public sealed class UserConfiguration : DomainEntityTypeConfiguration<User>
{
    public UserConfiguration(DatabaseProvider provider) : base(provider)
    {
    }

    public override void Configure(EntityTypeBuilder<User> builder)
    {
        base.Configure(builder);

        builder.HasKey(user => user.Id);

        builder.OwnsOne(user => user.Username, usernameBuilder =>
        {
            usernameBuilder
                .Property(username => username.Value)
                .IsRequired()
                .HasColumnName("username");

            usernameBuilder
                .Property(username => username.NormalizedValue)
                .IsRequired()
                .HasColumnName("normalized_username");

            usernameBuilder
                .HasIndex(username => username.Value)
                .IsUnique();
        });

        builder.OwnsOne(user => user.Name, nameBuilder =>
        {
            nameBuilder
                .Property(name => name.FirstName)
                .IsRequired()
                .HasColumnName("first_name");

            nameBuilder
                .Property(name => name.LastName)
                .IsRequired()
                .HasColumnName("last_name");

            nameBuilder
                .Property(name => name.NormalizedName)
                .IsRequired()
                .HasColumnName("normalized_name");
        });

        builder.OwnsOne(user => user.EmailAddress, emailBuilder =>
        {
            emailBuilder
                .Property(emailAddress => emailAddress.Value)
                .IsRequired()
                .HasColumnName("email");

            emailBuilder
                .Property(emailAddress => emailAddress.NormalizedValue)
                .IsRequired()
                .HasColumnName("normalized_email");

            emailBuilder
                .HasIndex(emailAddress => emailAddress.NormalizedValue)
                .IsUnique();

            emailBuilder
                .Property(emailAddress => emailAddress.IsConfirmed)
                .HasColumnName("email_confirmed");
        });

        builder.Property(user => user.Password)
            .HasConversion(
                password => password!.HashedValue ?? null,
                value => Password.FromHash(value));

        builder.Property(user => user.SecurityStamp)
            .IsRequired()
            .HasConversion(
                securityStamp => securityStamp.Value,
                value => SecurityStamp.FromValue(value));
    }
}