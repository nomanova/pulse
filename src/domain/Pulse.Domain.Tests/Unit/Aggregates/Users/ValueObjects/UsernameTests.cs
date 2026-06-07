using Bogus;
using Pulse.Domain.Aggregates.Users.ValueObjects;
using Xunit;

namespace Pulse.Domain.Tests.Unit.Aggregates.Users.ValueObjects;

public class UsernameTests
{
    [Fact]
    public void Create_WhenNull_ShouldReject()
    {
        var result = Username.Create(null);
        Assert.True(result.IsError);
    }

    [Fact]
    public void Create_WhenEmpty_ShouldReject()
    {
        var result = Username.Create("");
        Assert.True(result.IsError);
    }

    [Fact]
    public void Create_WhenTooLong_ShouldReject()
    {
        var username = new Faker().Random.String2(100);

        var result = Username.Create(username);
        Assert.True(result.IsError);
    }
    
    [Theory]
    [InlineData("validUser")]
    [InlineData("valid-user")]
    [InlineData("valid_user")]
    [InlineData("valid.user")]
    [InlineData("v")]
    [InlineData("vv")]
    [InlineData("admin")]
    [InlineData("admin_007")]
    public void Create_WhenValid_ShouldAccept(string value)
    {
        var result = Username.Create(value);
        Assert.False(result.IsError);
        Assert.Equal(value, result.Value.Value);
    }

    [Theory]
    [InlineData("-invalid")]
    [InlineData("_invalid")]
    [InlineData(".invalid")]
    [InlineData("invalid-")]
    [InlineData("invalid_")]
    [InlineData("invalid.")]
    [InlineData("-")]
    [InlineData("_")]
    [InlineData(".")]
    public void Create_WhenStartsOrEndsWithSpecialCharacter_ShouldReject(string value)
    {
        var result = Username.Create(value);
        Assert.True(result.IsError);
    }

    [Theory]
    [InlineData("user!")]
    [InlineData("user@")]
    [InlineData("user#")]
    [InlineData("user 123")]
    public void Create_WhenContainsForbiddenCharacters_ShouldReject(string value)
    {
        var result = Username.Create(value);
        Assert.True(result.IsError);
    }
}
