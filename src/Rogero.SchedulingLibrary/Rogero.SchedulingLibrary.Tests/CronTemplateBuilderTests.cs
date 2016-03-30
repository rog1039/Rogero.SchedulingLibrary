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

    public class CronSchedulerCallbackTests
    {
        private readonly TestScheduler _testScheduler = new TestScheduler();
        private readonly IDateTimeRepository _dateTimeRepository;
        private CronTemplate _cronTemplate;

        public CronSchedulerCallbackTests()
        {
            _dateTimeRepository = new DateTimeRepositoryRx(_testScheduler);
            _cronTemplate =
                new CronTemplateBuilder().WithMinutes(0, 30)
                    .WithAllHours()
                    .WithAllDaysOfMonth()
                    .WithAllMonths()
                    .WithAllDaysOfWeek()
                    .BuildCronTemplate();
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void RunOnceAtStart()
        {
            var callbackCount = 0;
            var scheduler = new CronSchedulerCallback(_dateTimeRepository, _testScheduler, _cronTemplate);
            Logger.LogAction = z => Debug.WriteLine(z);
            scheduler.Start(() =>
            {
                Debug.WriteLine($"Client notified:  {_dateTimeRepository.Now():O}");
                callbackCount++;
            });
            _testScheduler.AdvanceBy(TimeSpan.FromMinutes(35).Ticks);
            Thread.Sleep(10);
            callbackCount.Should().Be(1);
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void Run100Times()
        {
            var callbackCount = 0;
            var scheduler = new CronSchedulerCallback(_dateTimeRepository, _testScheduler, _cronTemplate);
            scheduler.Start(() =>
            {
                callbackCount++;
            });
            _testScheduler.AdvanceBy(TimeSpan.FromDays(2).Ticks);
            Thread.Sleep(1000);
            Debug.WriteLine($"Callback count: {callbackCount}");
        }

        [Fact()]
        [Trait("Category", "Slow")]
        public void Run10000Times()
        {
            var callbackCount = 0;
            var scheduler = new CronSchedulerCallback(_dateTimeRepository, _testScheduler, _cronTemplate);
            scheduler.Start(() =>
            {
                callbackCount++;
            });
            _testScheduler.AdvanceBy(TimeSpan.FromDays(365).Ticks);
            Thread.Sleep(1000);
            Debug.WriteLine($"Callback count: {callbackCount}");
        }
    }
}
