using System;
using Bogus;
using Pulse.Domain.Aggregates.Users.ValueObjects;
using Pulse.Domain.Common.Errors;
using Pulse.Domain.Tests.Framework;
using Xunit;

namespace Pulse.Domain.Tests.Tests.Aggregates.Users.ValueObjects;

public class PasswordTests
{
    private readonly FakePasswordHasher _passwordHasher = new();

    [Fact]
    public void Create_WhenValid_ShouldAccept()
    {
        // Arrange
        var passwordValue = new Faker().Random.String2(Password.MinLength, Password.MaxLength);

        // Act
        var result = Password.Create(passwordValue, _passwordHasher);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal(_passwordHasher.Hash(passwordValue), result.Value.HashedValue);
    }

    [Fact]
    public void Create_WhenNull_ShouldReject()
    {
        // Act
        var result = Password.Create(null, _passwordHasher);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains(BusinessErrors.User.PasswordRequired, result.Errors);
    }

    [Fact]
    public void Create_WhenEmpty_ShouldReject()
    {
        // Act
        var result = Password.Create(string.Empty, _passwordHasher);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains(BusinessErrors.User.PasswordRequired, result.Errors);
    }

    [Fact]
    public void Create_WhenTooShort_ShouldReject()
    {
        // Arrange
        var passwordValue = new Faker().Random.String2(Password.MinLength - 1);

        // Act
        var result = Password.Create(passwordValue, _passwordHasher);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains(BusinessErrors.User.PasswordTooShort, result.Errors);
    }

    [Fact]
    public void Create_WhenTooLong_ShouldReject()
    {
        // Arrange
        var passwordValue = new Faker().Random.String2(Password.MaxLength + 1);

        // Act
        var result = Password.Create(passwordValue, _passwordHasher);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains(BusinessErrors.User.PasswordTooLong, result.Errors);
    }

    [Fact]
    public void FromHash_ShouldCreatePasswordWithHashedValue()
    {
        // Arrange
        const string hashedValue = "some-hash";

        // Act
        var password = Password.FromHash(hashedValue);

        // Assert
        Assert.Equal(hashedValue, password.HashedValue);
    }

    [Fact]
    public void Create_WhenHasherIsNull_ShouldThrow()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => Password.Create("some-password", null!));
    }
}
