using Bogus;
using Pulse.Domain.Aggregates.Users;
using Pulse.Domain.Aggregates.Users.ValueObjects;
using Pulse.Domain.Common.Exceptions;
using Pulse.Domain.Tests.Shared.Fakers;
using Xunit;

namespace Pulse.Domain.Tests.Unit.Aggregates.Users;

public class UserTests
{
    private readonly Faker _faker = new();
    private readonly FakePasswordHasher _passwordHasher = new();

    [Fact]
    public void Create_WithValidUsername_ShouldSucceed()
    {
        // Arrange
        var username = _faker.Internet.UserName();

        // Act
        var user = User.Create(username);

        // Assert
        Assert.NotNull(user);
        Assert.NotNull(user.Id);
        Assert.Equal(username, user.Username.Value);
        Assert.NotNull(user.SecurityStamp);
        Assert.NotEqual(default, user.CreatedAt);
        Assert.Null(user.ModifiedAt);
    }

    [Fact]
    public void SetName_WithValidData_ShouldUpdateName()
    {
        // Arrange
        var user = User.Create(_faker.Internet.UserName());
        var firstName = _faker.Name.FirstName();
        var lastName = _faker.Name.LastName();

        // Act
        user.SetName(firstName, lastName);

        // Assert
        Assert.NotNull(user.Name);
        Assert.Equal(firstName, user.Name.FirstName);
        Assert.Equal(lastName, user.Name.LastName);
        Assert.NotNull(user.ModifiedAt);
    }

    [Fact]
    public void SetPassword_WithValidPassword_ShouldUpdatePasswordAndResetSecurityStamp()
    {
        // Arrange
        var user = User.Create(_faker.Internet.UserName());
        var initialSecurityStamp = user.SecurityStamp.Value;
        var password = _faker.Internet.Password(Password.MinLength);

        // Act
        user.SetPassword(password, _passwordHasher);

        // Assert
        Assert.NotNull(user.Password);
        Assert.Equal(_passwordHasher.Hash(password), user.Password.HashedValue);
        Assert.NotEqual(initialSecurityStamp, user.SecurityStamp.Value);
        Assert.NotNull(user.ModifiedAt);
    }

    [Fact]
    public void SetEmail_WithValidEmail_ShouldUpdateEmail()
    {
        // Arrange
        var user = User.Create(_faker.Internet.UserName());
        var email = _faker.Internet.Email();

        // Act
        user.SetEmail(email);

        // Assert
        Assert.NotNull(user.EmailAddress);
        Assert.Equal(email.ToLowerInvariant(), user.EmailAddress.Value.ToLowerInvariant());
        Assert.False(user.EmailAddress.IsConfirmed);
        Assert.NotNull(user.ModifiedAt);
    }

    [Fact]
    public void SetEmailConfirmed_WithEmailSet_ShouldConfirmEmail()
    {
        // Arrange
        var user = User.Create(_faker.Internet.UserName());
        user.SetEmail(_faker.Internet.Email());

        // Act
        user.SetEmailConfirmed();

        // Assert
        Assert.True(user.EmailAddress!.IsConfirmed);
        Assert.NotNull(user.ModifiedAt);
    }

    [Fact]
    public void SetEmailConfirmed_WithoutEmailSet_ShouldThrowDomainException()
    {
        // Arrange
        var user = User.Create(_faker.Internet.UserName());

        // Act & Assert
        Assert.Throws<DomainException>(user.SetEmailConfirmed);
    }

    [Fact]
    public void IsMatchingPassword_WithCorrectPassword_ShouldReturnTrue()
    {
        // Arrange
        var user = User.Create(_faker.Internet.UserName());
        var password = _faker.Internet.Password(Password.MinLength);
        user.SetPassword(password, _passwordHasher);

        // Act
        var result = user.IsMatchingPassword(password, _passwordHasher);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsMatchingPassword_WithIncorrectPassword_ShouldReturnFalse()
    {
        // Arrange
        var user = User.Create(_faker.Internet.UserName());
        var password = _faker.Internet.Password(Password.MinLength);
        user.SetPassword(password, _passwordHasher);

        // Act
        var result = user.IsMatchingPassword("wrong-password", _passwordHasher);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsMatchingPassword_WithNoPasswordSet_ShouldReturnFalse()
    {
        // Arrange
        var user = User.Create(_faker.Internet.UserName());

        // Act
        var result = user.IsMatchingPassword("any-password", _passwordHasher);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ToString_ShouldReturnFormattedString()
    {
        // Arrange
        var user = User.Create(_faker.Internet.UserName());
        var firstName = _faker.Name.FirstName();
        var lastName = _faker.Name.LastName();
        user.SetName(firstName, lastName);

        // Act
        var result = user.ToString();

        // Assert
        Assert.Contains(user.Id.Value, result);
        Assert.Contains(firstName, result);
        Assert.Contains(lastName, result);
    }

    [Fact]
    public void Create_WithInvalidUsername_ShouldThrowDomainException()
    {
        // Act & Assert
        Assert.Throws<DomainException>(() => User.Create(""));
    }

    [Fact]
    public void SetName_WithInvalidData_ShouldThrowDomainException()
    {
        // Arrange
        var user = User.Create(_faker.Internet.UserName());

        // Act & Assert
        Assert.Throws<DomainException>(() => user.SetName("", ""));
    }

    [Fact]
    public void SetPassword_WithInvalidPassword_ShouldThrowDomainException()
    {
        // Arrange
        var user = User.Create(_faker.Internet.UserName());
        var invalidPassword = _faker.Random.String2(Password.MinLength - 1);

        // Act & Assert
        Assert.Throws<DomainException>(() => user.SetPassword(invalidPassword, _passwordHasher));
    }

    [Fact]
    public void SetEmail_WithInvalidEmail_ShouldThrowDomainException()
    {
        // Arrange
        var user = User.Create(_faker.Internet.UserName());

        // Act & Assert
        Assert.Throws<DomainException>(() => user.SetEmail("invalid-email"));
    }
}
