using System;
using FluentAssertions;
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

        [Fact()]
        [Trait("Category", "Instant")]
        public void TestIntsToDaysOfWeekConversion()
        {
            var weekdays = "12345";
            var daysOfWeek = CronTemplateBuilder.GetDaysOfWeekEnumFromStringOfInts(weekdays);
            daysOfWeek.Should().Be(DaysOfWeek.MTWTF);

            var weekdays2 = "0123456";
            var daysOfWeek2 = CronTemplateBuilder.GetDaysOfWeekEnumFromStringOfInts(weekdays2);
            daysOfWeek2.Should().Be(DaysOfWeek.Everyday);

            var weekdays3 = "14";
            var daysOfWeek3 = CronTemplateBuilder.GetDaysOfWeekEnumFromStringOfInts(weekdays3);
            daysOfWeek3.Should().Be(DaysOfWeek.Monday | DaysOfWeek.Thursday);


            var weekdays4 = "141414";
            var daysOfWeek4 = CronTemplateBuilder.GetDaysOfWeekEnumFromStringOfInts(weekdays4);
            daysOfWeek4.Should().Be(DaysOfWeek.Monday | DaysOfWeek.Thursday);
        }
    }
}
