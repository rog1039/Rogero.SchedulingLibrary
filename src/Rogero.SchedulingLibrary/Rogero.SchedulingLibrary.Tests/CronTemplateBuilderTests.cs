using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

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

    public class CronTimeGeneratorTests
    {
        private readonly ITestOutputHelper _output;

        public CronTimeGeneratorTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestMethod1()
        {
            var cronTemplate = new CronTemplateBuilder()
                .WithMinutes(0, 20)
                .WithHours(1, 21)
                .WithAllDaysOfMonth()
                .WithAllMonths()
                .WithWeekDays()
                .BuildCronTemplate();

            var cronTime = new CronTime(cronTemplate, new DateTime(2014,01,01));
            var next100Dates = CronTimeGenerator.Generate(cronTime).Take(3000);
            foreach (var next100Date in next100Dates)
            {
                _output.WriteLine(next100Date.Time.ToDateTime().Value.ToString("MM/dd/yyyy hh:mm:ss tt ddd"));
            }
        }
    }
}
