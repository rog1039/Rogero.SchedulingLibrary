using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using Rogero.SchedulingLibrary.Infrastructure;
using Rogero.SchedulingLibrary.Scheduling;
using Rogero.SchedulingLibrary.Streams;
using Xunit;

namespace Rogero.SchedulingLibrary.Tests.Streams
{
    public class CronSchedulerStreamTests
    {
        readonly CronTimeStreamBase _simpleStreamBase;
        readonly CronTimeStreamBase _complexStreamBase;

        private readonly TestScheduler _testScheduler = new TestScheduler();
        private readonly IDateTimeRepository _dateTimeRepository;
        private readonly Scheduler _simpleScheduler;
        private readonly Scheduler _complexScheduler;

        public CronSchedulerStreamTests()
        {
            _dateTimeRepository = new DateTimeRepositoryRx(_testScheduler);

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

            _simpleStreamBase = new CronTimeStreamSimple(breakTemplate, DateTime.Now);
            _complexStreamBase = new CronTimeStreamComplex(DateTime.Now, breakTemplate, hourlyTemplate);

            _simpleScheduler = new Scheduler(_dateTimeRepository, _testScheduler, _simpleStreamBase, true);
            _complexScheduler = new Scheduler(_dateTimeRepository, _testScheduler, _complexStreamBase, true);
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void TestSimpleStream()
        {
            var callbackCount = 0;
            //Logger.LogAction = z => Debug.WriteLine(z);
            _simpleScheduler.Start((cronTime) =>
            {
                Debug.WriteLine($"Client notified:  {_dateTimeRepository.Now().ToString(DateTimeFormats.DatetimeFormatWithDayOfWeek)}");
                callbackCount++;
            });
            _testScheduler.AdvanceBy(TimeSpan.FromHours(91).Ticks);
            Debug.WriteLine($"Final Time: {_testScheduler.Now}");
            Thread.Sleep(100);
            callbackCount.Should().Be(24);
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void TestComplexStream()
        {
            var callbackCount = 0;
            Logger.LogAction = z => Debug.WriteLine(z);
            var cronTimeEvents = new List<CronTime>();
            _complexScheduler.Start((cronTime) =>
            {
                callbackCount++;
                cronTimeEvents.Add(cronTime);
            });

            _testScheduler.AdvanceBy(TimeSpan.FromHours(48).Ticks);
            Thread.Sleep(1000);
            callbackCount.Should().Be(22);
            Console.WriteLine("Printing received events");
            foreach (var cronTimeEvent in cronTimeEvents)
            {
                Debug.WriteLine($"{cronTimeEvent.DateTime.Value.ToString(DateTimeFormats.DatetimeFormat)}");
            }
            Console.WriteLine("Ending received events");

            foreach (var cronTime in _complexScheduler.UpcomingEvents.Take(11))
            {
                Debug.WriteLine($"{cronTime.DateTime.Value.ToString(DateTimeFormats.DatetimeFormat)}");
            }

            _testScheduler.AdvanceBy(TimeSpan.FromHours(48).Ticks);
            callbackCount.Should().Be(44);
        }
    }
}