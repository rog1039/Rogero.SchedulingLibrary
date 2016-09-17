using System;
using System.Collections.Generic;
using System.Linq;
using Rogero.SchedulingLibrary.Infrastructure;

namespace Rogero.SchedulingLibrary.Streams
{
    public static class CronScheduleParser
    {
        public static CronTimeStreamBase CreateCronTimeStream(DaysOfWeek daysOfWeek, string input, DateTime dateTime)
        {
            dateTime = dateTime == default(DateTime)
                ? DateTime.Now
                : dateTime;
            var cronTemplates = CreateCronTemplates(daysOfWeek, input);
            return new CronTimeStreamComplex(dateTime, cronTemplates.ToArray());
        }

        private static IEnumerable<CronTemplate> CreateCronTemplates(DaysOfWeek daysOfWeek, string input)
        {
            var times = GetTimes(input);
            var days = CronTemplateBuilder.GetDaysListFromEnum(daysOfWeek);
            var cronTemplates = CreateCronTemplates(days, times);
            return cronTemplates;
        }

        private static IList<TimeSpan> GetTimes(string input)
        {
            var times = input
                .Split(new[] {" ", ","}, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Select(s => DateStringParser.ParseTime(s).TimeOfDay)
                .ToList();
            return times;
        }

        private static IEnumerable<CronTemplate> CreateCronTemplates(IList<int> days, IList<TimeSpan> times)
        {
            foreach (var day in days)
            {
                foreach (var daysTemplate in CreateDaysTemplates(day, times))
                    yield return daysTemplate;
            }
        }

        private static IEnumerable<CronTemplate> CreateDaysTemplates(int day, IList<TimeSpan> times)
        {
            foreach (var time in times)
            {
                var isNextDay = time < times[0];
                var cronDay = isNextDay
                    ? (day+1) % 7
                    : day;
                yield return new CronTemplateBuilder()
                    .WithMinutes(time.Minutes)
                    .WithHours(time.Hours)
                    .WithAllDaysOfMonth()
                    .WithDaysOfWeek(cronDay)
                    .WithAllMonths()
                    .Build();
            }
        }
    }
}