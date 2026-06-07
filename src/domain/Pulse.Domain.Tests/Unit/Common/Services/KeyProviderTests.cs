using System.Collections.Generic;
using System.Linq;
using Pulse.Domain.Common.Services;
using Xunit;

namespace Pulse.Domain.Tests.Unit.Common.Services;

public class KeyProviderTests
{
    [Fact]
    public void New_ShouldReturnRandomString()
    {
        var keys = new List<string>();

        for (var i = 0; i < 10000; i++)
        {
            var key = KeyProvider.New();
            Assert.Equal(42, key.Length);

            keys.Add(key);
        }

        Assert.Equal(10000, keys.Distinct().Count());
    }
}