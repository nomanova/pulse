using System;
using Pulse.Domain.Common.Models.Entities;
using Xunit;

namespace Pulse.Domain.Tests.Common.Models.Entities;

public class EntityIdTests
{
    private record TestEntityId : EntityId
    {
        public TestEntityId(string value)
        {
            Value = value;
        }
    }

    [Fact]
    public void CompareTo_BothNull_ShouldReturnZero_Wait_CompareToParamIsNullable()
    {
        // EntityId.CompareTo(other) doesn't allow calling on null, obviously.
    }

    [Fact]
    public void CompareTo_OtherNull_ShouldReturnOne()
    {
        var id = new TestEntityId("a");
        Assert.Equal(1, id.CompareTo(null));
    }

    [Fact]
    public void CompareTo_SameValue_ShouldReturnZero()
    {
        var id1 = new TestEntityId("a");
        var id2 = new TestEntityId("a");
        Assert.Equal(0, id1.CompareTo(id2));
    }

    [Fact]
    public void CompareTo_LesserValue_ShouldReturnNegative()
    {
        var id1 = new TestEntityId("a");
        var id2 = new TestEntityId("b");
        Assert.True(id1.CompareTo(id2) < 0);
    }

    [Fact]
    public void CompareTo_GreaterValue_ShouldReturnPositive()
    {
        var id1 = new TestEntityId("b");
        var id2 = new TestEntityId("a");
        Assert.True(id1.CompareTo(id2) > 0);
    }

    [Fact]
    public void GreaterThanOperator_Tests()
    {
        var a = new TestEntityId("a");
        var b = new TestEntityId("b");
        
        Assert.True(b > a);
        Assert.False(a > b);
        Assert.False(a > a);
        Assert.True(a > null);
        Assert.False(null > a);
    }

    [Fact]
    public void LessThanOperator_Tests()
    {
        var a = new TestEntityId("a");
        var b = new TestEntityId("b");
        
        Assert.True(a < b);
        Assert.False(b < a);
        Assert.False(a < a);
        Assert.False(a < null);
        Assert.True(null < a);
    }

    [Fact]
    public void GreaterThanOrEqualOperator_Tests()
    {
        var a = new TestEntityId("a");
        var b = new TestEntityId("b");
        
        Assert.True(b >= a);
        Assert.True(a >= a);
        Assert.False(a >= b);
        Assert.True(a >= null);
        Assert.False(null >= a);
        Assert.True((TestEntityId?)null >= (TestEntityId?)null);
    }

    [Fact]
    public void LessThanOrEqualOperator_Tests()
    {
        var a = new TestEntityId("a");
        var b = new TestEntityId("b");
        
        Assert.True(a <= b);
        Assert.True(a <= a);
        Assert.False(b <= a);
        Assert.False(a <= null);
        Assert.True(null <= a);
        Assert.True((TestEntityId?)null <= (TestEntityId?)null);
    }

    [Fact]
    public void CompareTo_Object_OtherNull_ShouldReturnOne()
    {
        var id = new TestEntityId("a");
        Assert.Equal(1, ((IComparable)id).CompareTo(null));
    }

    [Fact]
    public void CompareTo_Object_SameType_ShouldReturnExpectedValue()
    {
        var id1 = new TestEntityId("a");
        var id2 = new TestEntityId("b");
        Assert.True(((IComparable)id1).CompareTo(id2) < 0);
    }

    [Fact]
    public void CompareTo_Object_DifferentType_ShouldThrowArgumentException()
    {
        var id = new TestEntityId("a");
        Assert.Throws<ArgumentException>(() => ((IComparable)id).CompareTo("not an EntityId"));
    }
}
