using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Rogero.SchedulingLibrary.Infrastructure;

namespace Rogero.SchedulingLibrary
{
    public partial class CronTemplateBuilder
    {
        public static CronTemplate ForDays(DaysOfWeek daysOfWeek, int hour, int minute)
        {
            var daysOfWeekList = GetDaysListFromEnum(daysOfWeek);
            var cronTemplate = new CronTemplateBuilder()
                .WithMinutes(minute)
                .WithHours(hour)
                .WithAllDaysOfMonth()
                .WithDaysOfWeek(daysOfWeekList.ToArray())
                .WithAllMonths()
                .Build();
            return cronTemplate;
        }

        public static CronTemplate ForDays(DaysOfWeek daysOfWeek, string hour, int minute)
        {
            var daysOfWeekList = GetDaysListFromEnum(daysOfWeek);
            var hourInt = GetHourIntFromHourString(hour);
            var cronTemplate = new CronTemplateBuilder()
                .WithMinutes(minute)
                .WithHours(hourInt)
                .WithAllDaysOfMonth()
                .WithDaysOfWeek(daysOfWeekList.ToArray())
                .WithAllMonths()
                .Build();
            return cronTemplate;
        }
        public static CronTemplate ForDays(DaysOfWeek daysOfWeek, string time)
        {
            var daysOfWeekList = GetDaysListFromEnum(daysOfWeek);
            var timeValue = DateStringParser.ParseTime(time);
            var cronTemplate = new CronTemplateBuilder()
                .WithMinutes(timeValue.Minute)
                .WithHours(timeValue.Hour)
                .WithAllDaysOfMonth()
                .WithDaysOfWeek(daysOfWeekList.ToArray())
                .WithAllMonths()
                .Build();
            return cronTemplate;
        }

        private static int GetHourIntFromHourString(string hour)
        {
            var amIndex = hour.IndexOf("a", StringComparison.OrdinalIgnoreCase);
            var pmIndex = hour.IndexOf("p", StringComparison.OrdinalIgnoreCase);
            if (amIndex > 0) return Int32.Parse(hour.Substring(0, hour.Length - amIndex).Trim());
            if (pmIndex > 0) return Int32.Parse(hour.Substring(0, hour.Length - pmIndex).Trim());
            throw new InvalidDataException($"Error parsing hour text of '{hour}'. Must be a number followed by a, p, am, or pm. Case is ignored.");
        }

        public static IList<int> GetDaysListFromEnum(DaysOfWeek daysOfWeek)
        {
            var days = new List<int>();
            if (daysOfWeek.HasFlag(DaysOfWeek.Sunday)) days.Add(0);
            if (daysOfWeek.HasFlag(DaysOfWeek.Monday)) days.Add(1);
            if (daysOfWeek.HasFlag(DaysOfWeek.Tuesday)) days.Add(2);
            if (daysOfWeek.HasFlag(DaysOfWeek.Wednesday)) days.Add(3);
            if (daysOfWeek.HasFlag(DaysOfWeek.Thursday)) days.Add(4);
            if (daysOfWeek.HasFlag(DaysOfWeek.Friday)) days.Add(5);
            if (daysOfWeek.HasFlag(DaysOfWeek.Saturday)) days.Add(6);
            return days;
        }
    }
}
