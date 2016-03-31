using System;
using FluentAssertions;
using Xunit;

namespace Rogero.SchedulingLibrary.Tests
{
    public class ToValidCronTimeTests
    {
        private CronTemplate _template;

        public ToValidCronTimeTests()
        {
            _template = new CronTemplateBuilder()
                .WithMinutes(10,30,50)
                .WithHours(6,12,18)
                .WithDaysOfMonth(5,15,25)
                .WithMonths(3,6,9)
                .WithAllDaysOfWeek()
                .Build();

        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void TimeAlreadyValid()
        {
            var time = new CronTime(_template, new DateTime(2016, 6, 15, 12, 10, 0));
            var validTime = CronTimeValidator.GetNextCronTimeThatFitsTheTemplate(time);
            validTime.HasNoValue.Should().BeTrue();
        }
        
        [Fact()]
        [Trait("Category", "Instant")]
        public void MonthBefore()
        {
            var time = new CronTime(_template, new DateTime(2016, 01, 31, 13, 0, 0));
            var expectedTime = new CronTime(_template, new DateTime(2016, 03, 05, 06, 10, 0));
            time.Should().Be(expectedTime);
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void MonthInBetween()
        {
            var time = new CronTime(_template, new DateTime(2016, 5, 31, 13, 0, 0));
            var expectedTime = new CronTime(_template, new DateTime(2016, 06, 05, 06, 10, 0));
            time.Should().Be(expectedTime);
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void MonthAfter()
        {
            var time = new CronTime(_template, new DateTime(2016, 10, 31, 13, 0, 0));
            var expectedTime = new CronTime(_template, new DateTime(2017, 03, 05, 06, 10, 0));
            time.Should().Be(expectedTime);
        }
        
        [Fact()]
        [Trait("Category", "Instant")]
        public void DayBefore()
        {
            var time = new CronTime(_template, new DateTime(2016, 03, 02, 13, 0, 0));
            var expectedTime = new CronTime(_template, new DateTime(2016, 03, 05, 06, 10, 0));
            time.Should().Be(expectedTime);
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void DayInBetween()
        {
            var time = new CronTime(_template, new DateTime(2016, 03, 12, 13, 0, 0));
            var expectedTime = new CronTime(_template, new DateTime(2016, 03, 15, 06, 10, 0));
            time.Should().Be(expectedTime);
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void DayAfter()
        {
            var time = new CronTime(_template, new DateTime(2016, 03, 31, 13, 0, 0));
            var expectedTime = new CronTime(_template, new DateTime(2016, 06, 05, 06, 10, 0));
            time.Should().Be(expectedTime);
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void HourBefore()
        {
            var time = new CronTime(_template, new DateTime(2016, 03, 05, 4, 0, 0));
            var expectedTime = new CronTime(_template, new DateTime(2016, 03, 05, 06, 10, 0));
            time.Should().Be(expectedTime);
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void HourInBetween()
        {
            var time = new CronTime(_template, new DateTime(2016, 03, 05, 13, 0, 0));
            var expectedTime = new CronTime(_template, new DateTime(2016, 03, 05, 18, 10, 0));
            time.Should().Be(expectedTime);
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void HourAfter()
        {
            var time = new CronTime(_template, new DateTime(2016, 03, 05, 21, 0, 0));
            var expectedTime = new CronTime(_template, new DateTime(2016, 03, 15, 06, 10, 0));
            time.Should().Be(expectedTime);
        }


        [Fact()]
        [Trait("Category", "Instant")]
        public void MinuteBefore()
        {
            var time = new CronTime(_template, new DateTime(2016, 03, 05, 06, 05, 0));
            var expectedTime = new CronTime(_template, new DateTime(2016, 03, 05, 06, 10, 0));
            time.Should().Be(expectedTime);
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void MinuteInBetween()
        {
            var time = new CronTime(_template, new DateTime(2016, 03, 05, 12, 25, 0));
            var expectedTime = new CronTime(_template, new DateTime(2016, 03, 05, 12, 30, 0));
            time.Should().Be(expectedTime);
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void MinuteAfter()
        {
            var time = new CronTime(_template, new DateTime(2016, 03, 05, 12, 55, 0));
            var expectedTime = new CronTime(_template, new DateTime(2016, 03, 05, 18, 10, 0));
            time.Should().Be(expectedTime);
        }
    }
}