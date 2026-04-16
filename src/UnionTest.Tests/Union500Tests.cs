using UnionTest;
using Xunit;

namespace UnionTest.Tests;

public class Union500Tests
{
    [Fact]
    public void Case501_AssignAndMatch()
    {
        Union500 u = new Case501(501);
        Assert.True(u.Value is Case501 { Value: 501 });
    }

    [Fact]
    public void Case750_AssignAndMatch()
    {
        Union500 u = new Case750(750);
        Assert.True(u.Value is Case750 { Value: 750 });
    }

    [Fact]
    public void Case1000_AssignAndMatch()
    {
        Union500 u = new Case1000(1000);
        Assert.True(u.Value is Case1000 { Value: 1000 });
    }

    [Theory]
    [InlineData(501)]
    [InlineData(600)]
    [InlineData(700)]
    [InlineData(800)]
    [InlineData(900)]
    [InlineData(1000)]
    public void SampleCases_AssignAndVerify(int caseNumber)
    {
        Union500 u = caseNumber switch
        {
            501 => new Case501(caseNumber),
            600 => new Case600(caseNumber),
            700 => new Case700(caseNumber),
            800 => new Case800(caseNumber),
            900 => new Case900(caseNumber),
            1000 => new Case1000(caseNumber),
            _ => throw new InvalidOperationException()
        };

        var matched = caseNumber switch
        {
            501 => u.Value is Case501 { Value: 501 },
            600 => u.Value is Case600 { Value: 600 },
            700 => u.Value is Case700 { Value: 700 },
            800 => u.Value is Case800 { Value: 800 },
            900 => u.Value is Case900 { Value: 900 },
            1000 => u.Value is Case1000 { Value: 1000 },
            _ => false
        };

        Assert.True(matched);
    }
}
