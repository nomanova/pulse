using System;
using System.Numerics;
using Pulse.Domain.Common.Services;
using Xunit;

namespace Pulse.Domain.Tests.Unit.Common.Services;

public sealed class Base36Tests
{
    [Theory]
    [InlineData("0", 0)]
    [InlineData("1", 1)]
    [InlineData("9", 9)]
    [InlineData("a", 10)]
    [InlineData("z", 35)]
    [InlineData("10", 36)]
    [InlineData("1z", 71)]
    [InlineData("zz", 1295)]
    [InlineData("A", 10)]
    [InlineData("Z", 35)]
    [InlineData("1Z", 71)]
    [InlineData("AbC", 13368)] // a*36^2 + b*36^1 + c*36^0 = 10*1296 + 11*36 + 12
    public void Decode_KnownValues_ReturnsExpected(string input, int expected)
    {
        var actual = Base36.Decode(input);

        Assert.Equal(new BigInteger(expected), actual);
    }

    [Theory]
    [InlineData(0, "")]
    [InlineData(1, "1")]
    [InlineData(9, "9")]
    [InlineData(10, "a")]
    [InlineData(35, "z")]
    [InlineData(36, "10")]
    [InlineData(71, "1z")]
    [InlineData(1295, "zz")]
    public void Encode_KnownValues_ReturnsExpected(int input, string expected)
    {
        var actual = Base36.Encode(new BigInteger(input));

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Encode_Negative_ThrowsArgumentOutOfRangeException()
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => Base36.Encode(new BigInteger(-1)));

        Assert.Equal("input", ex.ParamName);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(35)]
    [InlineData(36)]
    [InlineData(71)]
    [InlineData(1295)]
    public void EncodeThenDecode_RoundTrips_ForSmallValues(int value)
    {
        var encoded = Base36.Encode(new BigInteger(value));
        var decoded = Base36.Decode(encoded);

        Assert.Equal(new BigInteger(value), decoded);
    }

    [Fact]
    public void EncodeThenDecode_RoundTrips_ForLargeBigInteger()
    {
        // A value larger than Int64 to ensure BigInteger paths are exercised.
        var value = BigInteger.Parse("1234567890123456789012345678901234567890");

        var encoded = Base36.Encode(value);
        var decoded = Base36.Decode(encoded);

        Assert.Equal(value, decoded);
    }

    [Theory]
    [InlineData("!")]
    [InlineData("-")]
    [InlineData("_")]
    [InlineData("hello-world")]
    public void Decode_InvalidCharacters_CurrentBehavior_ProducesNegativeOrUnexpected(string input)
    {
        // NOTE: Base36.Decode currently uses CharList.IndexOf(c) and does not validate characters.
        // For invalid characters IndexOf returns -1, which affects the result.
        // This test documents the current behavior (so changes are deliberate later).
        var decoded = Base36.Decode(input);

        Assert.True(decoded < 0 || decoded != BigInteger.Zero);
    }
}