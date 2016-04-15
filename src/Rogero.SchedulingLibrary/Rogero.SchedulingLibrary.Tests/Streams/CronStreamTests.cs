using System;
using System.Diagnostics;
using System.Linq;
using FluentAssertions;
using Rogero.SchedulingLibrary.Generators;
using Rogero.SchedulingLibrary.Streams;
using Xunit;

namespace Rogero.SchedulingLibrary.Tests.Streams
{
    public class CronStreamTests
    {

        [Fact()]
        [Trait("Category", "Instant")]
        public void ScheduleTests()
        {
            CronTimeStreamBase cronStream = CronStream.CreateSchedule(DaysOfWeek.Weekdays, "6a, 7a, 8a, 9a,9:15a,9:30a,11a,12p,1p,2p,2:15p,2:30p,4p,5p");
            CronTimeStreamBase cronStream2 = CronStream.CreateSchedule(DaysOfWeek.SMTWT, "10:30p, 12:15a, 12:30a, 2:30a, 3a, 5a, 5:15a, 7a");
            CronTimeStreamBase cronStream3 = CronStream.CreateSchedule(DaysOfWeek.Friday, "9:30p, 11:15p, 11:30p, 1:30a, 2a, 4a, 4:15a, 6a");

            foreach (var time in cronStream.Take(200))
            {
                Debug.WriteLine(time.DateTime.Value.ToString("yyyy-MM-dd ddd  hh:mm:ss tt"));
            }

            foreach (var time in cronStream2.Take(200))
            {
                Debug.WriteLine(time.DateTime.Value.ToString("yyyy-MM-dd ddd  hh:mm:ss tt"));
            }

            foreach (var time in cronStream3.Take(200))
            {
                Debug.WriteLine(time.DateTime.Value.ToString("yyyy-MM-dd ddd  hh:mm:ss tt"));
            }
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void TestNextDaySchedule()
        {
            var stream = CronStream.CreateSchedule(DaysOfWeek.Monday | DaysOfWeek.Tuesday,
                                                   "10p, 3a",
                                                   new DateTime(2016, 04, 11, 01, 01, 00));
            var results = stream.Take(5).ToList();
            results[0].DateTime.Should().Be(new DateTime(2016, 04, 11, 22, 0, 0));
            results[1].DateTime.Should().Be(new DateTime(2016, 04, 12, 03, 0, 0));
            results[2].DateTime.Should().Be(new DateTime(2016, 04, 12, 22, 0, 0));
            results[3].DateTime.Should().Be(new DateTime(2016, 04, 13, 03, 0, 0));
            results[4].DateTime.Should().Be(new DateTime(2016, 04, 18, 22, 0, 0));

            foreach (var time in stream.Take(200))
            {
                Debug.WriteLine(time.DateTime.Value.ToString("yyyy-MM-dd ddd  hh:mm:ss tt"));
            }
        }
    }

    public class CronTimeStreamCombinationTests
    {
        private readonly DateTime _dateTime = new DateTime(2016, 01, 01, 0, 0, 1);

        [Fact()]
        [Trait("Category", "Instant")]
        public void SimpleTest()
        {
            var stream = CronStream.CreateSchedule(DaysOfWeek.Monday, "9p");
            var streamCombination = new CronTimeStreamCombination(_dateTime, stream);

            foreach (var time in streamCombination.Take(10))
            {
                Debug.WriteLine(time.DateTime.Value.ToString("yyyy-MM-dd ddd  hh:mm:ss tt"));
            }
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void SimpleTest2()
        {
            var everyMinute = new CronTemplateBuilder().WithEverything().EveryXMinutes(2).Build();
            var stream = new CronTimeStreamSimple(everyMinute, _dateTime);
            var streamCombination = new CronTimeStreamCombination(_dateTime, stream);
            
            foreach (var time in streamCombination.Take(200))
            {
                Debug.WriteLine(time.DateTime.Value.ToString("yyyy-MM-dd ddd  hh:mm:ss tt"));
            }
            var results = streamCombination.Take(5).ToList();
            results[0].Time.Minute.Should().Be(2);
            results[1].Time.Minute.Should().Be(4);
            results[2].Time.Minute.Should().Be(6);
            results[3].Time.Minute.Should().Be(8);
            results[4].Time.Minute.Should().Be(10);
        }

    }
}