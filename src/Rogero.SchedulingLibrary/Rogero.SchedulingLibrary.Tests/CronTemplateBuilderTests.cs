using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Rogero.SchedulingLibrary.Tests
{
    public class CronTemplateBuilderTests
    {
        [Fact()]
        [Trait("Category", "Instant")]
        public void ThrowIfTemplateNotTotallyFilledOut()
        {
            Assert.Throws<ArgumentNullException>(
                () => new CronTemplateBuilder().BuildCronTemplate());
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void BuildShouldWork()
        {
            var cronTemplate = new CronTemplateBuilder()
                .WithMinutes(0)
                .WithHours(4, 16)
                .WithDaysOfMonth(1, 10, 25)
                .WithAllMonths()
                .WithAllDaysOfWeek()
                .BuildCronTemplate();
        }
    }

    public class IntInListAnalysisResultTests_Count1
    {
        private List<int> _list;

        public IntInListAnalysisResultTests_Count1()
        {
            _list = new List<int>() { 30 };
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void LessThan()
        {
            var result = new IntInListAnalysisResult(10, _list);
            result.IntInListResultEnum.Should().Be(IntInListResultEnum.Before);
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void Equal()
        {
            var result = new IntInListAnalysisResult(30, _list);
            result.IntInListResultEnum.Should().Be(IntInListResultEnum.First);
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void After()
        {
            var result = new IntInListAnalysisResult(40, _list);
            result.IntInListResultEnum.Should().Be(IntInListResultEnum.After);
        }
    }

    public class IntInListAnalysisResultTests_Count2
    {
        private List<int> _list;

        public IntInListAnalysisResultTests_Count2()
        {
            _list = new List<int>() { 10, 20 };
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void Before()
        {
            var result = new IntInListAnalysisResult(5, _list);
            result.IntInListResultEnum.Should().Be(IntInListResultEnum.Before);
            result.Index.ShouldBeEquivalentTo(null);
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void First()
        {
            var result = new IntInListAnalysisResult(10, _list);
            result.IntInListResultEnum.Should().Be(IntInListResultEnum.First);
            result.Index.Should().Be(0);
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void InBetween()
        {
            var result = new IntInListAnalysisResult(12, _list);
            result.IntInListResultEnum.Should().Be(IntInListResultEnum.InBetween);
            result.NextClosestUpperIndex.Should().Be(1);
            result.Index.ShouldBeEquivalentTo(null);
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void Last()
        {
            var result = new IntInListAnalysisResult(20, _list);
            result.IntInListResultEnum.Should().Be(IntInListResultEnum.Last);
            result.Index.Should().Be(1);
        }


        [Fact()]
        [Trait("Category", "Instant")]
        public void After()
        {
            var result = new IntInListAnalysisResult(40, _list);
            result.IntInListResultEnum.Should().Be(IntInListResultEnum.After);
            result.Index.ShouldBeEquivalentTo(null);
        }
    }

    public class IntInListAnalysisResultTests_Count3
    {
        private List<int> _list;

        public IntInListAnalysisResultTests_Count3()
        {
            _list = new List<int>() { 10, 20, 30 };
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void Before()
        {
            var result = new IntInListAnalysisResult(5, _list);
            result.IntInListResultEnum.Should().Be(IntInListResultEnum.Before);
            result.Index.ShouldBeEquivalentTo(null);
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void First()
        {
            var result = new IntInListAnalysisResult(10, _list);
            result.IntInListResultEnum.Should().Be(IntInListResultEnum.First);
            result.Index.Should().Be(0);
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void InBetween()
        {
            var result = new IntInListAnalysisResult(12, _list);
            result.IntInListResultEnum.Should().Be(IntInListResultEnum.InBetween);
            result.NextClosestUpperIndex.Should().Be(1);
            result.Index.ShouldBeEquivalentTo(null);
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void MiddleIndex()
        {
            var result = new IntInListAnalysisResult(20, _list);
            result.IntInListResultEnum.Should().Be(IntInListResultEnum.InBetween);
            result.Index.ShouldBeEquivalentTo(1);
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void InBetween2()
        {
            var result = new IntInListAnalysisResult(22, _list);
            result.IntInListResultEnum.Should().Be(IntInListResultEnum.InBetween);
            result.NextClosestUpperIndex.Should().Be(2);
            result.Index.ShouldBeEquivalentTo(null);
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void Last()
        {
            var result = new IntInListAnalysisResult(30, _list);
            result.IntInListResultEnum.Should().Be(IntInListResultEnum.Last);
            result.Index.Should().Be(2);
        }


        [Fact()]
        [Trait("Category", "Instant")]
        public void After()
        {
            var result = new IntInListAnalysisResult(40, _list);
            result.IntInListResultEnum.Should().Be(IntInListResultEnum.After);
            result.Index.ShouldBeEquivalentTo(null);
        }
    }

    public class ToValidCronTimeTests
    {
        private CronTemplate _template;

        public ToValidCronTimeTests()
        {
            _template = new CronTemplateBuilder()
                .WithMinutes(10,30,50)
                .WithHours(6,12,18)
                .WithDaysOfMonth(5,15,25)
                .WithMonths(3,6,9)
                .WithAllDaysOfWeek()
                .BuildCronTemplate();

        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void TimeAlreadyValid()
        {
            var time = new CronTime(_template, new DateTime(2016, 6, 15, 12, 10, 0));
            var validTime = CronTimeIncrementor.ToValidCronTime(time);
            time.Should().Be(validTime);
        }
        
        [Fact()]
        [Trait("Category", "Instant")]
        public void MonthBefore()
        {
            var time = new CronTime(_template, new DateTime(2016, 01, 31, 13, 0, 0));
            var validTime = CronTimeIncrementor.ToValidCronTime(time);
            var expectedTime = new CronTime(_template, new DateTime(2016, 03, 05, 06, 10, 0));
            validTime.Should().Be(expectedTime);
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void MonthInBetween()
        {
            var time = new CronTime(_template, new DateTime(2016, 5, 31, 13, 0, 0));
            var validTime = CronTimeIncrementor.ToValidCronTime(time);
            var expectedTime = new CronTime(_template, new DateTime(2016, 06, 05, 06, 10, 0));
            validTime.Should().Be(expectedTime);
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void MonthAfter()
        {
            var time = new CronTime(_template, new DateTime(2016, 10, 31, 13, 0, 0));
            var validTime = CronTimeIncrementor.ToValidCronTime(time);
            var expectedTime = new CronTime(_template, new DateTime(2017, 03, 05, 06, 10, 0));
            validTime.Should().Be(expectedTime);
        }
        
        [Fact()]
        [Trait("Category", "Instant")]
        public void DayBefore()
        {
            var time = new CronTime(_template, new DateTime(2016, 03, 02, 13, 0, 0));
            var validTime = CronTimeIncrementor.ToValidCronTime(time);
            var expectedTime = new CronTime(_template, new DateTime(2016, 03, 05, 06, 10, 0));
            validTime.Should().Be(expectedTime);
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void DayInBetween()
        {
            var time = new CronTime(_template, new DateTime(2016, 03, 12, 13, 0, 0));
            var validTime = CronTimeIncrementor.ToValidCronTime(time);
            var expectedTime = new CronTime(_template, new DateTime(2016, 03, 15, 06, 10, 0));
            validTime.Should().Be(expectedTime);
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void DayAfter()
        {
            var time = new CronTime(_template, new DateTime(2016, 03, 31, 13, 0, 0));
            var validTime = CronTimeIncrementor.ToValidCronTime(time);
            var expectedTime = new CronTime(_template, new DateTime(2016, 06, 05, 06, 10, 0));
            validTime.Should().Be(expectedTime);
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void HourBefore()
        {
            var time = new CronTime(_template, new DateTime(2016, 03, 05, 4, 0, 0));
            var validTime = CronTimeIncrementor.ToValidCronTime(time);
            var expectedTime = new CronTime(_template, new DateTime(2016, 03, 05, 06, 10, 0));
            validTime.Should().Be(expectedTime);
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void HourInBetween()
        {
            var time = new CronTime(_template, new DateTime(2016, 03, 05, 13, 0, 0));
            var validTime = CronTimeIncrementor.ToValidCronTime(time);
            var expectedTime = new CronTime(_template, new DateTime(2016, 03, 05, 18, 10, 0));
            validTime.Should().Be(expectedTime);
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void HourAfter()
        {
            var time = new CronTime(_template, new DateTime(2016, 03, 05, 21, 0, 0));
            var validTime = CronTimeIncrementor.ToValidCronTime(time);
            var expectedTime = new CronTime(_template, new DateTime(2016, 03, 15, 06, 10, 0));
            validTime.Should().Be(expectedTime);
        }
    }

    public class CronTimeIncrementorTests
    {
        private CronTemplate template =
            new CronTemplateBuilder()
                .WithMinutes(0, 5, 20)
                .WithHours(4, 12, 18)
                .WithAllDaysOfMonth()
                .WithAllDaysOfWeek()
                .WithAllMonths()
                .BuildCronTemplate();

        private CronTime time;

        public CronTimeIncrementorTests()
        {
            time = new CronTime(template, new DateTime(2016, 1, 1, 6, 10, 10));
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void MethodName()
        {
            var nextTime = time.GetNextEnsureValidDateTime();
            nextTime.Time.Should().Be(new Time(20, 12, 1, 1, 2016));
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void ConvertIncomingCronTimToValidCronTime()
        {
            var date = new CronTime(template, new DateTime(2016, 01, 01, 13, 10, 0));
            var nextValid = CronTimeIncrementor.ConvertIncomingCronTimeToValidCronTime(date);
            nextValid.Time.ToDateTime().Should().Be(new DateTime(2016, 01, 01, 18, 20, 0));
        }
    }

    public class CronTimeGeneratorTests
    {
        private readonly ITestOutputHelper _output;
        private CronTemplate _cronTemplate;
        private CronTime _cronTime;

        public CronTimeGeneratorTests(ITestOutputHelper output)
        {
            _output = output;
            _cronTemplate = new CronTemplateBuilder()
                .WithMinutes(0, 20)
                .WithHours(1, 21)
                .WithAllDaysOfMonth()
                .WithAllMonths()
                .WithWeekDays()
                .BuildCronTemplate();
            _cronTime = new CronTime(_cronTemplate, new DateTime(2014, 01, 01));
        }

        [Fact]
        public void GenerateSeveralThousandUnsafe()
        {
            var nextDates = CronTimeGenerator.Generate(_cronTime).Take(3000);
            foreach (var nextDate in nextDates)
            {
                _output.WriteLine(nextDate.Time.ToDateTime().Value.ToString("MM/dd/yyyy hh:mm:ss tt ddd"));
            }
        }

        [Fact]
        public void GenerateForLesserOf_LimitByCount()
        {
            var nextDates = CronTimeGenerator.GenerateForLessorOf(_cronTime, TimeSpan.FromDays(99999), 5);
            foreach (var nextDate in nextDates)
            {
                _output.WriteLine(nextDate.Time.ToDateTime().Value.ToString("MM/dd/yyyy hh:mm:ss tt ddd"));
            }
        }

        [Fact]
        public void GenerateForLesserOf_LimitByTimeSpan()
        {
            var nextDates = CronTimeGenerator.GenerateForLessorOf(_cronTime, TimeSpan.FromDays(3), 99999);
            foreach (var nextDate in nextDates)
            {
                _output.WriteLine(nextDate.Time.ToDateTime().Value.ToString("MM/dd/yyyy hh:mm:ss tt ddd"));
            }
        }

        [Fact]
        public void CreateByLesserOfTimeSpanOrCountWithMinimum()
        {
            var nextDates = CronTimeGenerator.CreateByLesserOfTimeSpanOrCountWithMinimum(_cronTime, TimeSpan.FromSeconds(3), 10, 99999);
            foreach (var nextDate in nextDates)
            {
                _output.WriteLine(nextDate.Time.ToDateTime().Value.ToString("MM/dd/yyyy hh:mm:ss tt ddd"));
            }
        }

        [Fact]
        public void EveryMinute()
        {
            var template =
                new CronTemplateBuilder().WithAllMinutes()
                    .WithAllHours()
                    .WithAllDaysOfMonth()
                    .WithAllMonths()
                    .WithAllDaysOfWeek()
                    .BuildCronTemplate();
            DateTime shouldBeDate;
            var startingDateTime = shouldBeDate = new DateTime(2014, 01, 01);
            var time = new CronTime(template, startingDateTime);
            var nextDates = CronTimeGenerator.CreateByLesserOfTimeSpanOrCountWithMinimum(time, TimeSpan.FromSeconds(3), 400, 99999);
            foreach (var nextDate in nextDates)
            {
                _output.WriteLine(nextDate.Time.ToDateTime().Value.ToString("MM/dd/yyyy hh:mm:ss tt ddd"));
                shouldBeDate = shouldBeDate.AddMinutes(1);
                nextDate.Time.ToDateTime().Should().Be(shouldBeDate);
            }
        }
    }
}
