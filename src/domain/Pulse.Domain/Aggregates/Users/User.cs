using Pulse.Domain.Aggregates.Users.Services;
using Pulse.Domain.Aggregates.Users.ValueObjects;
using Pulse.Domain.Common.Errors;
using Pulse.Domain.Common.Models.Entities;
using Pulse.Domain.Common.Models.ValueObjects;
using Pulse.Domain.Common.Services;

namespace Pulse.Domain.Aggregates.Users;

public sealed record UserId : EntityId<UserId, User>;

public class User : DomainEntity<UserId>
{
    public ObjectName Username { get; private set; } = null!;

    public Name? Name { get; private set; }
    
    public EmailAddress? EmailAddress { get; private set; }

    public Password? Password { get; private set; }

    public SecurityStamp SecurityStamp { get; private set; } = null!;

    private User()
    {
    }

    private User(
        UserId id,
        ObjectName username) : base(id)
    {
        Username = username;
        SecurityStamp = SecurityStamp.Create();
    }

    public static User Create(string? username)
    {
        var usernameValue = ObjectName.Create(username).Assert();
        var id = IdentityProvider.New<UserId>();

        var user = new User(id, usernameValue);

        user.SetCreated();

        return user;
    }

    public void SetName(string? firstName, string? lastName)
    {
        Name = Name.Create(firstName, lastName).Assert();
        
        SetModified();
    }

    public void SetPassword(string? password, IUserPasswordHasher passwordHasher)
    {
        Password = Password.Create(password, passwordHasher).Assert();
        SecurityStamp.Reset();

        SetModified();
    }

    public void SetEmail(string? emailAddress)
    {
        EmailAddress = EmailAddress.Create(emailAddress).Assert();

        SetModified();
    }

    public void SetEmailConfirmed()
    {
        DomainErrors.User.EmailAddressRequired.Assert(() => EmailAddress != null);

        EmailAddress!.SetConfirmed();
        SetModified();
    }

    public bool IsMatchingPassword(string? password, IUserPasswordHasher passwordHasher)
    {
        if (Password == null || string.IsNullOrEmpty(password))
        {
            return false;
        }

        var hash = Password.HashedValue;
        return hash != null && passwordHasher.Verify(password, hash);
    }

    public override string ToString()
    {
        return $"[{Id.Value}] {Name}";
    }
}