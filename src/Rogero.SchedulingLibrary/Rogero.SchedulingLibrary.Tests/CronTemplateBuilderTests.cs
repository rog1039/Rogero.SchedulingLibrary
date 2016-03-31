using System;
using System.Diagnostics;
using System.Threading;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using Rogero.SchedulingLibrary.Infrastructure;
using Rogero.SchedulingLibrary.Scheduling;
using Xunit;

namespace Rogero.SchedulingLibrary.Tests
{
    public class CronTemplateBuilderTests
    {
        [Fact()]
        [Trait("Category", "Instant")]
        public void ThrowIfTemplateNotTotallyFilledOut()
        {
            Assert.Throws<ArgumentNullException>(
                () => new CronTemplateBuilder().Build());
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
                .Build();
        }
    }

    public class CronSchedulerCallbackTests
    {
        private readonly TestScheduler _testScheduler = new TestScheduler();
        private readonly IDateTimeRepository _dateTimeRepository;
        private CronTemplate _cronTemplate;
        private int _callbackCount;
        private CronSchedulerCallback _scheduler;

        public CronSchedulerCallbackTests()
        {
            _dateTimeRepository = new DateTimeRepositoryRx(_testScheduler);
            _cronTemplate =
                new CronTemplateBuilder().WithMinutes(0, 30)
                    .WithAllHours()
                    .WithAllDaysOfMonth()
                    .WithAllMonths()
                    .WithAllDaysOfWeek()
                    .Build();

            _callbackCount = 0;
            _scheduler = new CronSchedulerCallback(_dateTimeRepository, _testScheduler, _cronTemplate);
            Logger.LogAction = z => Debug.WriteLine(z);
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void RunOnceAtStart()
        {
            _scheduler.Start(() =>
            {
                Debug.WriteLine($"Client notified:  {_dateTimeRepository.Now():O}");
                _callbackCount++;
            });
            _testScheduler.AdvanceBy(TimeSpan.FromMinutes(35).Ticks);
            Thread.Sleep(100);
            _callbackCount.Should().Be(2);
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void Run100Times()
        {
            _scheduler.Start(() =>
            {
                _callbackCount++;
            });
            _testScheduler.AdvanceBy(TimeSpan.FromDays(2).Ticks);
            Thread.Sleep(1000);
            Debug.WriteLine($"Callback count: {_callbackCount}");
        }

        [Fact()]
        [Trait("Category", "Slow")]
        public void Run10000Times()
        {
            _scheduler.Start(() =>
            {
                _callbackCount++;
            });
            _testScheduler.AdvanceBy(TimeSpan.FromDays(365).Ticks);
            Thread.Sleep(1000);
            Debug.WriteLine($"Callback count: {_callbackCount}");
        }
    }
}
