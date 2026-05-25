using Bogus;
using Pulse.Domain.Aggregates.Users.ValueObjects;
using Xunit;

namespace Pulse.Domain.Tests.Tests.Aggregates.Users.ValueObjects;

public class NameTests
{
    private readonly Faker _faker = new();

    [Fact]
    public void Create_WhenValid_ShouldAccept()
    {
        // Arrange
        var firstName = _faker.Name.FirstName();
        var lastName = _faker.Name.LastName();

        // Act
        var result = Name.Create(firstName, lastName);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal(firstName, result.Value.FirstName);
        Assert.Equal(lastName, result.Value.LastName);
    }

    [Theory]
    [InlineData(null, "Doe")]
    [InlineData("", "Doe")]
    [InlineData(" ", "Doe")]
    [InlineData("John", null)]
    [InlineData("John", "")]
    [InlineData("John", " ")]
    public void Create_WhenFirstOrLastNameIsNullOrWhitespace_ShouldReject(string? firstName, string? lastName)
    {
        // Act
        var result = Name.Create(firstName, lastName);

        // Assert
        Assert.True(result.IsError);
    }

    [Fact]
    public void Create_WhenFirstNameTooLong_ShouldReject()
    {
        // Arrange
        var firstName = _faker.Random.String2(101);
        var lastName = _faker.Name.LastName();

        // Act
        var result = Name.Create(firstName, lastName);

        // Assert
        Assert.True(result.IsError);
    }

    [Fact]
    public void Create_WhenLastNameTooLong_ShouldReject()
    {
        // Arrange
        var firstName = _faker.Name.FirstName();
        var lastName = _faker.Random.String2(101);

        // Act
        var result = Name.Create(firstName, lastName);

        // Assert
        Assert.True(result.IsError);
    }

    [Fact]
    public void FullName_ShouldBeCombinationOfFirstAndLastName()
    {
        // Arrange
        var firstName = _faker.Name.FirstName();
        var lastName = _faker.Name.LastName();
        var name = Name.Create(firstName, lastName).Value;

        // Assert
        Assert.Equal($"{firstName} {lastName}", name.FullName);
    }

    [Theory]
    [InlineData("John", "Doe", "johndoe")]
    [InlineData("Jôhn", "Döe", "johndoe")]
    [InlineData("John-Paul", "Sartre", "johnpaulsartre")]
    [InlineData("  John  ", "  Doe  ", "johndoe")]
    public void NormalizedName_ShouldBeCorrectlyNormalized(string firstName, string lastName, string expectedNormalized)
    {
        // Act
        var result = Name.Create(firstName, lastName);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal(expectedNormalized, result.Value.NormalizedName);
    }
}
