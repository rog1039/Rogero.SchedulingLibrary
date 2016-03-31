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

namespace Rogero.SchedulingLibrary.Tests.Scheduling
{
    public class CronSchedulerStreamReactiveTests
    {
        private readonly TestScheduler _testScheduler = new TestScheduler();
        private readonly IDateTimeRepository _dateTimeRepository;
        private readonly CronTimeStreamBase _simpleStreamBase;
        private readonly SchedulerReactive _schedulerReactive;

        public CronSchedulerStreamReactiveTests()
        {
            _dateTimeRepository = new DateTimeRepositoryRx(_testScheduler);

            var breakTemplate = new CronTemplateBuilder()
                .WithMinutes(0, 15, 30)
                .WithHours(9, 14)
                .WithAllDaysOfMonth()
                .WithDaysOfWeek(1, 2, 3, 4, 5)
                .WithAllMonths()
                .Build();
            
            _simpleStreamBase = new CronTimeStreamSimple(breakTemplate, _dateTimeRepository.Now());
            _schedulerReactive = new SchedulerReactive(
                _dateTimeRepository, _testScheduler, _simpleStreamBase);
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void GetFirstEvent()
        {
            int callCount = 0;
            CronTime lastEvent = null;
            Logger.LogAction = s => Debug.WriteLine(s);
            _schedulerReactive.SchedulerObservable.Subscribe(cronTime =>
            {
                callCount++;
                lastEvent = cronTime;
                Console.WriteLine(cronTime.DateTime.Value.ToString("yyyy-MM-dd  hh:mm:ss tt"));
            });

            _testScheduler.AdvanceBy(TimeSpan.FromHours(9).Ticks);

            Thread.Sleep(1000);
            callCount.Should().Be(1);
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void TestSeriesOfCronTimes()
        {
            int callCount = 0;
            CronTime lastEvent = null;
            Logger.LogAction = s => Debug.WriteLine(s);
            var cronTimeEvents = new List<CronTime>();
            _schedulerReactive.SchedulerObservable.Subscribe(cronTime =>
            {
                callCount++;
                cronTimeEvents.Add(cronTime);
                lastEvent = cronTime;
            });
            
            _testScheduler.AdvanceBy(TimeSpan.FromDays(1).Ticks);
            Thread.Sleep(1000);

            foreach (var cronTimeEvent in cronTimeEvents)
            {
                Console.WriteLine(cronTimeEvent.DateTime.Value.ToString("yyyy-MM-dd  hh:mm:ss tt"));
            }

            callCount.Should().Be(6);
            _schedulerReactive.LastFiredSchedule.DateTime.Value.Should().Be(new DateTime(1, 1, 1, 14, 30, 0));

            Console.WriteLine("Starting upcoming event listing");
            foreach (var cronTime in _schedulerReactive.UpcomingEvents.Take(15000))
            {
                Debug.WriteLine($"{cronTime.DateTime.Value.ToString("yyyy-MM-dd  hh:mm:ss tt ddd")}");
            }
            _schedulerReactive.LastFiredSchedule.DateTime.Value.Should().Be(new DateTime(1, 1, 1, 14, 30, 0));
        }
    }
}
