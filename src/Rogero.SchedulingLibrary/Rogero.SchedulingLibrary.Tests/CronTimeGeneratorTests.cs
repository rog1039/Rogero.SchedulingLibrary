using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using Rogero.SchedulingLibrary.Generators;
using Rogero.SchedulingLibrary.Infrastructure;
using Rogero.SchedulingLibrary.Scheduling;
using Rogero.SchedulingLibrary.Streams;
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

    public class CronTimeAggregateGeneratorTests
    {
        private IEnumerable<CronTime> _nextTimes;

        public CronTimeAggregateGeneratorTests()
        {
            var breakTemplate = new CronTemplateBuilder()
                .WithMinutes(0, 15, 30)
                .WithHours(9, 14)
                .WithAllDaysOfMonth()
                .WithDaysOfWeek(1, 2, 3, 4, 5)
                .WithAllMonths()
                .BuildCronTemplate();

            var hourlyTemplate = new CronTemplateBuilder()
                .WithMinutes(0)
                .WithHours(11, 12, 13, 16, 17)
                .WithAllDaysOfMonth()
                .WithDaysOfWeek(1, 2, 3, 4, 5)
                .WithAllMonths()
                .BuildCronTemplate();

            _nextTimes = CronTimeAggregateGenerator
                .Generate(new DateTime(2016, 01, 01), breakTemplate, hourlyTemplate);
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void Generate()
        {
            foreach (var nextTime in _nextTimes.Take(30000))
            {
                Console.WriteLine(nextTime.DateTime.Value.ToString("yyyy-MM-dd  hh:mm:ss tt  dddd"));
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
            foreach (var nextTime in _nextTimes.ForLesserOfWithMinimum(TimeSpan.FromDays(1), 3000,11))
            {
                Console.WriteLine(nextTime.DateTime.Value.ToString("yyyy-MM-dd  hh:mm:ss tt  dddd"));
            }
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void GenerateForLesserOf2WithMin_BoundedByCount()
        {
            foreach (var nextTime in _nextTimes.ForLesserOfWithMinimum(TimeSpan.FromDays(7), 5, 3))
            {
                Console.WriteLine(nextTime.DateTime.Value.ToString("yyyy-MM-dd  hh:mm:ss tt  dddd"));
            }
        }
    }

    public class CronSchedulerStreamTests
    {
        CronTimeStreamBase _simpleStreamBase;
        CronTimeStreamBase _complexStreamBase;

        private readonly TestScheduler _testScheduler = new TestScheduler();
        private readonly IDateTimeRepository _dateTimeRepository;
        private CronSchedulerStream _simpleScheduler;
        private CronSchedulerStream _complexScheduler;

        public CronSchedulerStreamTests()
        {
            _dateTimeRepository = new DateTimeRepositoryRx(_testScheduler);

            var breakTemplate = new CronTemplateBuilder()
                .WithMinutes(0, 15, 30)
                .WithHours(9, 14)
                .WithAllDaysOfMonth()
                .WithDaysOfWeek(1, 2, 3, 4, 5)
                .WithAllMonths()
                .BuildCronTemplate();

            var hourlyTemplate = new CronTemplateBuilder()
                .WithMinutes(0)
                .WithHours(11, 12, 13, 16, 17)
                .WithAllDaysOfMonth()
                .WithDaysOfWeek(1, 2, 3, 4, 5)
                .WithAllMonths()
                .BuildCronTemplate();

            _simpleStreamBase = new CronTimeStreamSimple(breakTemplate, DateTime.Now);
            _complexStreamBase = new CronTimeStreamComplex(DateTime.Now, breakTemplate, hourlyTemplate);

            _simpleScheduler = new CronSchedulerStream(_dateTimeRepository, _testScheduler, _simpleStreamBase);
            _complexScheduler = new CronSchedulerStream(_dateTimeRepository, _testScheduler, _complexStreamBase);
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void TestSimpleStream()
        {
            var callbackCount = 0;
            Logger.LogAction = z => Debug.WriteLine(z);
            _simpleScheduler.Start((cronTime) =>
            {
                //Debug.WriteLine($"Client notified:  {_dateTimeRepository.Now():O}");
                callbackCount++;
            });
            _testScheduler.AdvanceBy(TimeSpan.FromHours(91).Ticks);
            Debug.WriteLine(_testScheduler.Now);
            Thread.Sleep(100);
            callbackCount.Should().Be(23);
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void TestStreamWithEnd()
        {
            var callbackCount = 0;
            Logger.LogAction = z => Debug.WriteLine(z);
            _simpleScheduler.Start((cronTime) =>
            {
                //Debug.WriteLine($"Client notified:  {_dateTimeRepository.Now():O}");
                callbackCount++;
            });
            _testScheduler.AdvanceBy(TimeSpan.FromHours(91).Ticks);
            Debug.WriteLine(_testScheduler.Now);
            Thread.Sleep(100);
            callbackCount.Should().Be(23);
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void TestComplexStream()
        {
            var callbackCount = 0;
            Logger.LogAction = z => Debug.WriteLine(z);
            _complexScheduler.Start((cronTime) =>
            {
                callbackCount++;
            });

            _testScheduler.AdvanceBy(TimeSpan.FromHours(48).Ticks);
            Thread.Sleep(1000);
            callbackCount.Should().Be(22);

            foreach (var cronTime in _complexScheduler.UpcomingEvents.Take(11))
            {
                Debug.WriteLine($"{cronTime.DateTime.Value.ToString("yyyy-MM-dd  hh:mm:ss tt")}");
            }

            _testScheduler.AdvanceBy(TimeSpan.FromHours(48).Ticks);
            callbackCount.Should().Be(44);
        }
    }
}