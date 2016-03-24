using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace Rogero.SchedulingLibrary.Tests
{
    //public class IntInListAnalysisResultTests_Count3
    //{
    //    private List<int> _list;

    //    public IntInListAnalysisResultTests_Count3()
    //    {
    //        _list = new List<int>() { 10, 20, 30 };
    //    }

    //    [Fact()]
    //    [Trait("Category", "Instant")]
    //    public void Before()
    //    {
    //        var result = new IntInListAnalysisResult(5, _list);
    //        result.IntInListResultEnum.Should().Be(IntInListResultEnum.Before);
    //        result.Index.ShouldBeEquivalentTo(null);
    //    }

    //    [Fact()]
    //    [Trait("Category", "Instant")]
    //    public void First()
    //    {
    //        var result = new IntInListAnalysisResult(10, _list);
    //        result.IntInListResultEnum.Should().Be(IntInListResultEnum.First);
    //        result.Index.Should().Be(0);
    //    }

    //    [Fact()]
    //    [Trait("Category", "Instant")]
    //    public void InBetween()
    //    {
    //        var result = new IntInListAnalysisResult(12, _list);
    //        result.IntInListResultEnum.Should().Be(IntInListResultEnum.InBetween);
    //        result.NextClosestUpperIndex.Should().Be(1);
    //        result.Index.ShouldBeEquivalentTo(null);
    //    }

    //    [Fact()]
    //    [Trait("Category", "Instant")]
    //    public void MiddleIndex()
    //    {
    //        var result = new IntInListAnalysisResult(20, _list);
    //        result.IntInListResultEnum.Should().Be(IntInListResultEnum.MiddleIndex);
    //        result.Index.ShouldBeEquivalentTo(1);
    //    }

    //    [Fact()]
    //    [Trait("Category", "Instant")]
    //    public void InBetween2()
    //    {
    //        var result = new IntInListAnalysisResult(22, _list);
    //        result.IntInListResultEnum.Should().Be(IntInListResultEnum.InBetween);
    //        result.NextClosestUpperIndex.Should().Be(2);
    //        result.Index.ShouldBeEquivalentTo(null);
    //    }

    //    [Fact()]
    //    [Trait("Category", "Instant")]
    //    public void Last()
    //    {
    //        var result = new IntInListAnalysisResult(30, _list);
    //        result.IntInListResultEnum.Should().Be(IntInListResultEnum.Last);
    //        result.Index.Should().Be(2);
    //    }


    //    [Fact()]
    //    [Trait("Category", "Instant")]
    //    public void After()
    //    {
    //        var result = new IntInListAnalysisResult(40, _list);
    //        result.IntInListResultEnum.Should().Be(IntInListResultEnum.After);
    //        result.Index.ShouldBeEquivalentTo(null);
    //    }
    //}
}