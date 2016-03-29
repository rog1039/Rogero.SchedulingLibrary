using System;
using System.Linq;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Rogero.SchedulingLibrary.Tests
{
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
            var nextDates = CronTimeGenerator.CreateByLesserOfTimeSpanOrCountWithMinimum(time, TimeSpan.FromSeconds(3), 4000, 99999);
            foreach (var nextDate in nextDates)
            {
                _output.WriteLine(nextDate.Time.ToDateTime().Value.ToString("MM/dd/yyyy hh:mm:ss tt ddd"));
                shouldBeDate = shouldBeDate.AddMinutes(1);
                nextDate.Time.ToDateTime().Should().Be(shouldBeDate);
            }
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void TestNextCronTimeWhenStartingCronTimeIsInvalid()
        {
            var nextTimes = CronTimeGenerator.Generate(
                new CronTime(
                    new CronTemplateBuilder().WithMinutes(0, 45)
                        .WithAllHours()
                        .WithAllDaysOfMonth()
                        .WithAllDaysOfWeek()
                        .WithAllMonths()
                        .BuildCronTemplate(), new DateTime(2016, 03, 29, 09, 30, 0), false)).Take(5);

            foreach (var nextTime in nextTimes)
            {
                Console.WriteLine(nextTime.DateTime.Value.ToString("O"));
            }

            nextTimes.First().DateTime.Value.Should().Be(new DateTime(2016, 03, 29, 09, 45, 0));
        }
    }
}