using Pulse.Domain.Aggregates.Users;
using Pulse.Tests.Shared.Fakers;

namespace Pulse.Tests.Shared.Builders;

public sealed class UserBuilder : IBuilder<User>
{
    private string? _username;
    private string? _firstName;
    private string? _lastName;
    private string? _email;
    private bool _isEmailConfirmed;
    private string? _password;

    private UserBuilder()
    {
    }

    public static UserBuilder New()
    {
        return Gregory();
    }

    public static UserBuilder Gregory()
    {
        return new UserBuilder()
            .WithUsername("gregory")
            .WithName("Gregory", "House")
            .WithEmail("gregory@house.com")
            .SetEmailConfirmed()
            .WithPassword("Gregory123456");
    }

    public UserBuilder WithUsername(string username)
    {
        _username = username;
        return this;
    }

    public UserBuilder WithName(string firstName, string lastName)
    {
        _firstName = firstName;
        _lastName = lastName;
        return this;
    }

    public UserBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public UserBuilder SetEmailConfirmed()
    {
        _isEmailConfirmed = true;
        return this;
    }

    public UserBuilder WithPassword(string password)
    {
        _password = password;
        return this;
    }

    public User Build()
    {
        var user = User.Create(_username);

        if (!string.IsNullOrEmpty(_email))
        {
            user.SetEmail(_email);
        }

        if (_isEmailConfirmed)
        {
            user.SetEmailConfirmed();
        }

        if (!string.IsNullOrEmpty(_password))
        {
            user.SetPassword(_password, FakePasswordHasher.Default);
        }

        if (!string.IsNullOrEmpty(_firstName) && !string.IsNullOrEmpty(_lastName))
        {
            user.SetName(_firstName, _lastName);
        }

        return user;
    }
}