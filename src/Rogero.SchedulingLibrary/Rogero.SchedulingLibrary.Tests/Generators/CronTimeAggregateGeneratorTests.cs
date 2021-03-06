using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FluentAssertions;
using Rogero.SchedulingLibrary.Generators;
using Rogero.SchedulingLibrary.Streams;
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
                PrintTime(nextTime);
            }
        }

        private static void PrintTime(CronTime nextTime)
        {
            Console.WriteLine(nextTime.DateTime.Value.ToString("yyyy-MM-dd  hh:mm:ss.fff tt  dddd"));
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void GenerateForLesserOf_BoundedByCount()
        {
            var results = _nextTimes.ForLesserOf(TimeSpan.FromDays(7), 3).ToList();
            foreach (var nextTime in results)
            {
                PrintTime(nextTime);
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
                PrintTime(nextTime);
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
                PrintTime(nextTime);
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
                PrintTime(nextTime);
                count++;
            }
            count.Should().Be(5);
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void DontDropUpcomingCronTimesWhenTwoCronTemplatesGenerateTheSameTime()
        {
            var every3Minutes = new CronTemplateBuilder().WithEverything().WithSeconds(0).EveryXMinutes(3).Build();
            var every4Minutes = new CronTemplateBuilder().WithEverything().WithSeconds(0).EveryXMinutes(4).Build();

            var nextTimes = CronTimeGenerator
                .Generate(DateTime.MinValue, every3Minutes, every4Minutes)
                .Take(100)
                .ToList();

            foreach (var nextTime in nextTimes)
            {
                PrintTime(nextTime);
            }

            var expectedValues3 = Enumerable.Range(1, 20).Select(z => z*3).Where(z => z < 60).ToList();
            var expectedValues4 = Enumerable.Range(1, 20).Select(z => z*4).Where(z => z < 60).ToList();
            var expectedValues = new SortedSet<int>(expectedValues3.Union(expectedValues4)).Take(100).ToList();

            for (int i = 0; i < expectedValues.Count; i++)
            {
                nextTimes[i].Time.Minute.Should().Be(expectedValues[i]);
            }
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void CronTimeGeneratorTests()
        {
            DateTime _dateTime = new DateTime(2016, 01, 01, 0, 0, 1);
            var stream = (CronTimeStreamComplex)CronStream.CreateSchedule(DaysOfWeek.Monday, "9p");
            var template = stream.CronTemplates.First();
            var times = CronTimeGenerator.Generate(_dateTime, template).Take(10).ToList();

            Debug.WriteLine(times[0].DateTime.Value.ToString("yyyy-MM-dd ddd  hh:mm:ss tt"));

            times[0].DateTime.Value.Should().Be(new DateTime(2016, 01, 04, 21, 0, 0));
            times[1].DateTime.Value.Should().Be(new DateTime(2016, 01, 11, 21, 0, 0));
        }
    }
}