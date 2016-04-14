using System;
using FluentAssertions;
using Rogero.SchedulingLibrary.Infrastructure;
using Xunit;

namespace Rogero.SchedulingLibrary.Tests.Infrastructure
{
    public class DateParsingTests
    {
        [Fact()]
        [Trait("Category", "Instant")]
        public void VariousTests()
        {
            Test("4a", new DateTime(1, 1, 1, 4, 0, 0));
            Test("4am", new DateTime(1, 1, 1, 4, 0, 0));
            Test("4p", new DateTime(1, 1, 1, 16, 0, 0));
            Test("4pm", new DateTime(1, 1, 1, 16, 0, 0));
            Test("4:15a", new DateTime(1, 1, 1, 4, 15, 0));
            Test("4:15am", new DateTime(1, 1, 1, 4, 15, 0));
            Test("4:15p", new DateTime(1, 1, 1, 16, 15, 0));
            Test("4:15pm", new DateTime(1, 1, 1, 16, 15, 0));
            Test("12a", new DateTime(1, 1, 1, 0, 0, 0));
            Test("12p", new DateTime(1, 1, 1, 12, 0, 0));
            Test("11a", new DateTime(1, 1, 1, 11, 0, 0));
            Test("11p", new DateTime(1, 1, 1, 23, 0, 0));

        }

        private void Test(string s, DateTime dateTime)
        {
            var time = DateStringParser.ParseTime(s);
            time.TimeOfDay.Should().Be(dateTime.TimeOfDay);
        }
    }
}