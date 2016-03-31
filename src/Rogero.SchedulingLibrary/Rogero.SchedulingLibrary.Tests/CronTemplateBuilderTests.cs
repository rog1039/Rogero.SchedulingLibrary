using System;
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
}
