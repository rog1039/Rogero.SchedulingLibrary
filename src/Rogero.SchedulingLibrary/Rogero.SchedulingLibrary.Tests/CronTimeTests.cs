using System;
using FluentAssertions;
using Xunit;

namespace Rogero.SchedulingLibrary.Tests
{
    public class CronTimeTests
    {
        private CronTemplate template =
            new CronTemplateBuilder()
                .WithMinutes(0, 5, 20)
                .WithHours(4, 12, 18)
                .WithAllDaysOfMonth()
                .WithAllDaysOfWeek()
                .WithAllMonths()
                .BuildCronTemplate();

        private CronTemplate mondayTemplate =
            new CronTemplateBuilder()
                .WithMinutes(0)
                .WithHours(8, 17)
                .WithAllDaysOfMonth()
                .WithDaysOfWeek(1)
                .WithAllMonths()
                .BuildCronTemplate();

        private CronTime time;

        public CronTimeTests()
        {
            time = new CronTime(template, new DateTime(2016, 1, 1, 6, 10, 10));
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void CreatingACronTimeShouldResultInAValidTime()
        {
            time.Time.Should().Be(new Time(new DateTime(2016,01,01,12,0,0)));
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void ConvertIncomingCronTimToValidCronTime()
        {
            var date = new CronTime(template, new DateTime(2016, 01, 01, 13, 10, 0));
            date.Time.ToDateTime().Should().Be(new DateTime(2016, 01, 01, 18, 0, 0));
        }
        
        [Fact()]
        [Trait("Category", "Instant")]
        public void TestFilteringByDayOfWeek()
        {
            var cronTime = new CronTime(mondayTemplate, new DateTime(2016, 01, 01, 13, 10, 0));
            for (int i = 0; i < 1000; i++)
            {
                //date.DateTime.DayOfWeek.Should().Be(DayOfWeek.Monday);
                Console.WriteLine($"{cronTime.DateTime.Value:R}");
                cronTime = cronTime.GetNext();
            }
        }
    }
}