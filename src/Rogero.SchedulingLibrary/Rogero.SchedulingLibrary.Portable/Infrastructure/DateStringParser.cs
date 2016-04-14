using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace Rogero.SchedulingLibrary.Infrastructure
{
    public static class DateStringParser
    {
        public static DateTime ParseTime(string time)
        {
            var timeRegex = new Regex("(?<hour>\\d{1,2})(?::(?<minute>\\d{1,2}))?\\s?(?<ampm>am|pm|a|p)");
            var hour = time.MatchRegex(timeRegex, "hour");
            var minute = time.MatchRegex(timeRegex, "minute");
            var ampm = time.MatchRegex(timeRegex, "ampm");

            var hourHasValue = hour.Length > 0;
            var ampmHasValue = ampm.Length > 0;
            var valid = hourHasValue && ampmHasValue;
            if (!valid)
                throw new InvalidDataException(
                    $"Input time was not parsable, {time}, must be in form hh:[mm](am/pm/a/p)");

            var hourValue = Int32.Parse(hour);
            var minuteValue = String.IsNullOrWhiteSpace(minute)
                ? 0
                : Int32.Parse(minute);
            var ampmValue = ampm.IndexOf("a", StringComparison.OrdinalIgnoreCase) >= 0
                ? "AM"
                : "PM";

            if (hourValue == 12) hourValue = 0;
            if (ampmValue == "PM") hourValue += 12;

            try
            {
                var dateTime = new DateTime(1, 1, 1, hourValue, minuteValue, 0);
                //Debug.WriteLine($"String: {time} converted to: {dateTime.ToString("hh:mm tt")}");
                return dateTime;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Hour: {hourValue}, Minute: {minuteValue}");
                Debug.WriteLine(e.Message);
                throw e;
            }
        }
    }
}