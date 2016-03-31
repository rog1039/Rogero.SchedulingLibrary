using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Rogero.SchedulingLibrary.Generators;
using Xunit;

namespace Rogero.SchedulingLibrary.Tests.Generators
{
    public class CronTimeAggregateGeneratorTests
    {
        private readonly IEnumerable<CronTime> _nextTimes;

        public CronTimeAggregateGeneratorTests()
        {
            var breakTemplate = new CronTemplateBuilder()
                .WithMinutes(0, 15, 30)
                .WithHours(9, 14)
                .WithAllDaysOfMonth()
                .WithDaysOfWeek(1, 2, 3, 4, 5)
                .WithAllMonths()
                .Build();

            var hourlyTemplate = new CronTemplateBuilder()
                .WithMinutes(0)
                .WithHours(11, 12, 13, 16, 17)
                .WithAllDaysOfMonth()
                .WithDaysOfWeek(1, 2, 3, 4, 5)
                .WithAllMonths()
                .Build();

            _nextTimes = CronTimeGenerator
                .Generate(new DateTime(2016, 01, 01), breakTemplate, hourlyTemplate);
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void Generate()
        {
            foreach (var nextTime in _nextTimes.Take(30000))
            {
                //Console.WriteLine(nextTime.DateTime.Value.ToString("yyyy-MM-dd  hh:mm:ss tt  dddd"));
            }
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void GenerateForTime()
        {
            foreach (var nextTime in _nextTimes.ForTime(TimeSpan.FromDays(7)))
            {
                Console.WriteLine(nextTime.DateTime.Value.ToString("yyyy-MM-dd  hh:mm:ss tt  dddd"));
            }
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void GenerateForLesserOf_BoundedByCount()
        {
            var results = _nextTimes.ForLesserOf(TimeSpan.FromDays(7), 3).ToList();
            foreach (var nextTime in results)
            {
                Console.WriteLine(nextTime.DateTime.Value.ToString("yyyy-MM-dd  hh:mm:ss tt  dddd"));
            }
            results.Count.Should().Be(3);
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void GenerateForLesserOf_BoundedByTime()
        {
            var results = _nextTimes.ForLesserOf(TimeSpan.FromDays(1), 30).ToList();
            foreach (var nextTime in results)
            {
                Console.WriteLine(nextTime.DateTime.Value.ToString("yyyy-MM-dd  hh:mm:ss tt  dddd"));
            }
            results.Count.Should().Be(11);
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void GenerateForLesserOf2WithMin_BoundedByTime()
        {
            int count = 0;
            foreach (var nextTime in _nextTimes.ForLesserOfWithMinimum(TimeSpan.FromDays(3.9), count: 3000, minReturned: 11))
            {
                Console.WriteLine(nextTime.DateTime.Value.ToString("yyyy-MM-dd  hh:mm:ss tt  dddd"));
                count++;
            }
            count.Should().Be(22);
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void GenerateForLesserOf2WithMin_BoundedByCount()
        {
            int count = 0;
            foreach (var nextTime in _nextTimes.ForLesserOfWithMinimum(TimeSpan.FromDays(7), count: 5, minReturned: 3))
            {
                Console.WriteLine(nextTime.DateTime.Value.ToString("yyyy-MM-dd  hh:mm:ss tt  dddd"));
                count++;
            }
            count.Should().Be(5);
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void DontDropUpcomingCronTimesWhenTwoCronTemplatesGenerateTheSameTime()
        {
            var every3Minutes = new CronTemplateBuilder().WithEverything().EveryXMinutes(3).Build();
            var every4Minutes = new CronTemplateBuilder().WithEverything().EveryXMinutes(4).Build();

            var nextTimes = CronTimeGenerator
                .Generate(DateTime.MinValue, every3Minutes, every4Minutes)
                .Take(100)
                .ToList();

            foreach (var nextTime in nextTimes)
            {
                Console.WriteLine(nextTime.DateTime.Value.ToString("yyyy-MM-dd  hh:mm:ss tt  dddd"));
            }

            var expectedValues3 = Enumerable.Range(0, 100).Select(z => z*3).Where(z => z < 60).ToList();
            var expectedValues4 = Enumerable.Range(0, 100).Select(z => z*4).Where(z => z < 60).ToList();
            var expectedValues = new SortedSet<int>(expectedValues3.Union(expectedValues4)).Take(100).ToList();

            for (int i = 0; i < expectedValues.Count; i++)
            {
                nextTimes[i].Time.Minute.Should().Be(expectedValues[i]);
            }
        }
    }
}