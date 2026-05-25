using Bogus;
using Pulse.Domain.Aggregates.Users.ValueObjects;
using Xunit;

namespace Pulse.Domain.Tests.Tests.Aggregates.Users.ValueObjects;

public class SecurityStampTests
{
    private readonly Faker _faker = new();

    [Fact]
    public void Create_ShouldGenerateNonEmptyValue()
    {
        // Act
        var securityStamp = SecurityStamp.Create();

        // Assert
        Assert.NotNull(securityStamp.Value);
        Assert.NotEmpty(securityStamp.Value);
    }

    [Fact]
    public void FromValue_ShouldReturnSecurityStampWithCorrectValue()
    {
        // Arrange
        var expectedValue = _faker.Random.AlphaNumeric(32);

        // Act
        var securityStamp = SecurityStamp.FromValue(expectedValue);

        // Assert
        Assert.Equal(expectedValue, securityStamp.Value);
    }

    [Fact]
    public void Reset_ShouldGenerateNewValue()
    {
        // Arrange
        var securityStamp = SecurityStamp.Create();
        var initialValue = securityStamp.Value;

        // Act
        securityStamp.Reset();

        // Assert
        Assert.NotEqual(initialValue, securityStamp.Value);
        Assert.NotNull(securityStamp.Value);
        Assert.NotEmpty(securityStamp.Value);
    }

    [Fact]
    public void Equals_ShouldReturnTrue_WhenValuesAreEqual()
    {
        // Arrange
        var value = "equal-value";
        var stamp1 = SecurityStamp.FromValue(value);
        var stamp2 = SecurityStamp.FromValue(value);

        // Act & Assert
        Assert.Equal(stamp1, stamp2);
        Assert.True(stamp1 == stamp2);
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenValuesAreDifferent()
    {
        // Arrange
        var stamp1 = SecurityStamp.Create();
        var stamp2 = SecurityStamp.Create();

        // Act & Assert
        Assert.NotEqual(stamp1, stamp2);
        Assert.False(stamp1 == stamp2);
    }
}
