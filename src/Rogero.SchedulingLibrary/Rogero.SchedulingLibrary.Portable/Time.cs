using System;
using System.Linq;

namespace Rogero.SchedulingLibrary
{
    public partial class Time : IEquatable<Time>, IComparable<Time>
    {
        public int Year { get; }
        public int Month { get; }
        public int Day { get; }
        public int Hour { get; }
        public int Minute { get; }
        public int Second { get; }

        public Time(int year, int month, int day, int hour, int minute, int second = 0)
        {
            Second = second;
            Minute = minute;
            Hour = hour;
            Day = day;
            Month = month;
            Year = year;
        }

        public Time(DateTime datetime)
        {
            //datetime = RoundUpToNearesetMinute(datetime);
            datetime = RoundUpToNearestSecond(datetime);
            Second = datetime.Second;
            Minute = datetime.Minute;
            Hour = datetime.Hour;
            Day = datetime.Day;
            Month = datetime.Month;
            Year = datetime.Year;
        }

        private static DateTime RoundUpToNearestSecond(DateTime datetime)
        {
            if (datetime.Millisecond == 0) return datetime;

            datetime = datetime.AddMilliseconds(1000 - datetime.Millisecond);
            return datetime;
        }

        private static DateTime RoundUpToNearesetMinute(DateTime datetime)
        {
            if (datetime.Second == 0) return datetime;

            datetime = datetime.AddSeconds(60 - datetime.Second);
            return datetime;
        }

        public Time ChangeSecond(int second, CronTemplate template)
        {
            return new Time(Year, Month, Day, Hour, Minute, second);
        }

        public Time ChangeMinute(int minute, CronTemplate template)
        {
            return new Time(Year, Month, Day, Hour, minute, template.Seconds.First());
        }

        public Time ChangeHour(int hour, CronTemplate template)
        {
            return new Time(Year, Month, Day, hour, template.Minutes.First(), template.Seconds.First());
        }

        public Time ChangeDay(int day, CronTemplate template)
        {
            return new Time(Year, Month, day, template.Hours.First(), template.Minutes.First(), template.Seconds.First());
        }

        public Time ChangeMonth(int month, CronTemplate template)
        {
            return new Time(Year, month, template.DaysOfMonth.First(), template.Hours.First(), template.Minutes.First(), template.Seconds.First());
        }

        public Time ChangeYear(int year, CronTemplate template)
        {
            return new Time(year, template.Months.First(), template.DaysOfMonth.First(), template.Hours.First(), template.Minutes.First(), template.Seconds.First());
        }

        public DateTime? ToDateTime()
        {
            try
            {
                var date = new DateTime(Year, Month, Day, Hour, Minute, Second);
                return date;
            }
            catch (Exception)
            {
                return null;
            }
        }
        
    }
}