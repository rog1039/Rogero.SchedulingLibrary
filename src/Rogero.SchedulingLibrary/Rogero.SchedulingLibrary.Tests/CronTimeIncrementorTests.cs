using System;
using FluentAssertions;
using Xunit;

namespace Rogero.SchedulingLibrary.Tests
{
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
            nextTime.Time.Should().Be(new Time(new DateTime(2016,01,01,12,0,0)));
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void ConvertIncomingCronTimToValidCronTime()
        {
            var date = new CronTime(template, new DateTime(2016, 01, 01, 13, 10, 0));
            var nextValid = CronTimeIncrementor.ToValidCronTime(date);
            nextValid.CronTime.Time.ToDateTime().Should().Be(new DateTime(2016, 01, 01, 18, 0, 0));
        }
    }
}