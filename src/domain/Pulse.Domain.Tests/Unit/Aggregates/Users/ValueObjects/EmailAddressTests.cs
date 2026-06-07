using Bogus;
using Pulse.Domain.Aggregates.Users.ValueObjects;
using Xunit;

namespace Pulse.Domain.Tests.Unit.Aggregates.Users.ValueObjects;

public class EmailAddressTests
{
    [Fact]
    public void Create_WhenNull_ShouldReject()
    {
        var result = EmailAddress.Create(null);
        Assert.True(result.IsError);
    }

    [Fact]
    public void Create_WhenEmpty_ShouldReject()
    {
        var result = EmailAddress.Create("");
        Assert.True(result.IsError);
    }

    [Fact]
    public void Create_WhenTooLong_ShouldReject()
    {
        var prefix = new Faker().Random.String2(300);
        var email = $"{prefix}@example.com";

        var result = EmailAddress.Create(email);
        Assert.True(result.IsError);
    }

    [Fact]
    public void Create_WhenInvalidFormat_ShouldReject()
    {
        // Note that only requirement is the presence of an '@' sign
        const string email = "email.com";

        var result = EmailAddress.Create(email);
        Assert.True(result.IsError);
    }

    [Fact]
    public void Create_WhenInvalidCharacters_ShouldReject()
    {
        const string email = "Françoise@email.com";

        var result = EmailAddress.Create(email);
        Assert.True(result.IsError);
    }

    [Fact]
    public void Create_WhenValid_ShouldNormalize()
    {
        const string email = "TEST@Test.Com";
        var result = EmailAddress.Create(email);

        Assert.False(result.IsError);
        Assert.Equal("TEST@Test.Com", result.Value.Value);
        Assert.Equal("test@test.com", result.Value.NormalizedValue);
    }

    [Fact]
    public void Create_WhenValid_ShouldNotBeConfirmed()
    {
        const string email = "test@test.com";
        var result = EmailAddress.Create(email);

        Assert.False(result.IsError);
        Assert.False(result.Value.IsConfirmed);
    }

    [Fact]
    public void SetConfirmed_WhenInvoked_ShouldMutateState()
    {
        const string email = "test@test.com";
        var result = EmailAddress.Create(email);

        result.Value.SetConfirmed();
        Assert.True(result.Value.IsConfirmed);
    }
}