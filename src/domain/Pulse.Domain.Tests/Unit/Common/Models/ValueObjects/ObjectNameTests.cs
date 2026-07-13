using Bogus;
using Pulse.Domain.Common.Models.ValueObjects;
using Xunit;

namespace Pulse.Domain.Tests.Unit.Common.Models.ValueObjects;

public class ObjectNameTests
{
    [Fact]
    public void Create_WhenNull_ShouldReject()
    {
        var result = ObjectName.Create(null);

        Assert.True(result.IsError);
    }

    [Fact]
    public void Create_WhenEmpty_ShouldReject()
    {
        var result = ObjectName.Create("");

        Assert.True(result.IsError);
    }

    [Fact]
    public void Create_WhenWhitespace_ShouldReject()
    {
        var result = ObjectName.Create(" ");

        Assert.True(result.IsError);
    }

    [Fact]
    public void Create_WhenTooLong_ShouldReject()
    {
        var name = new Faker().Random.String2(ObjectName.MaxLength + 1, "abcdefghijklmnopqrstuvwxyz0123456789");

        var result = ObjectName.Create(name);

        Assert.True(result.IsError);
    }

    [Theory]
    [InlineData("valid")]
    [InlineData("valid-name")]
    [InlineData("v")]
    [InlineData("vv")]
    [InlineData("name123")]
    [InlineData("123name")]
    [InlineData("123")]
    [InlineData("acme-hq")]
    public void Create_WhenValid_ShouldAccept(string value)
    {
        var result = ObjectName.Create(value);

        Assert.False(result.IsError);
        Assert.Equal(value, result.Value.Value);
    }

    [Theory]
    [InlineData("-invalid")]
    [InlineData("invalid-")]
    [InlineData("-")]
    public void Create_WhenStartsOrEndsWithHyphen_ShouldReject(string value)
    {
        var result = ObjectName.Create(value);

        Assert.True(result.IsError);
    }

    [Theory]
    [InlineData("name!")]
    [InlineData("name@")]
    [InlineData("name#")]
    [InlineData("name_123")]
    [InlineData("name.123")]
    [InlineData("name 123")]
    [InlineData("Name")]
    public void Create_WhenContainsForbiddenCharacters_ShouldReject(string value)
    {
        var result = ObjectName.Create(value);

        Assert.True(result.IsError);
    }

    [Theory]
    [InlineData("valid-name", "validname")]
    [InlineData("acme-hq", "acmehq")]
    [InlineData("name123", "name123")]
    [InlineData("1-2-3", "123")]
    public void NormalizedValue_ShouldRemoveHyphens(string value, string expectedNormalizedValue)
    {
        var result = ObjectName.Create(value);

        Assert.False(result.IsError);
        Assert.Equal(expectedNormalizedValue, result.Value.NormalizedValue);
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        const string value = "valid-name";

        var result = ObjectName.Create(value);

        Assert.False(result.IsError);
        Assert.Equal(value, result.Value.ToString());
    }
}