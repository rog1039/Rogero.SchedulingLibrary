using System;
using System.Linq;

namespace Rogero.SchedulingLibrary
{
    public class Time
    {
        public int Minute { get; }
        public int Hour { get; }
        public int Day { get; }
        public int Month { get; }
        public int Year { get; }

        public Time(int minute, int hour, int day, int month, int year)
        {
            Minute = minute;
            Hour = hour;
            Day = day;
            Month = month;
            Year = year;
        }

        public Time(DateTime datetime)
        {
            Minute = datetime.Minute;
            Hour = datetime.Hour;
            Day = datetime.Day;
            Month = datetime.Month;
            Year = datetime.Year;
        }

        public Time ChangeMinute(int minute, CronTemplate template)
        {
            return new Time(minute, Hour, Day, Month, Year);
        }

        public Time ChangeHour(int hour, CronTemplate template)
        {
            return new Time(template.Minutes.First(), hour, Day, Month, Year);
        }

        public Time ChangeDay(int day, CronTemplate template)
        {
            return new Time(template.Minutes.First(), template.Hours.First(), day, Month, Year);
        }

        public Time ChangeMonth(int month, CronTemplate template)
        {
            return new Time(template.Minutes.First(), template.Hours.First(), template.DaysOfMonth.First(), Month, Year);
        }

        public DateTime? ToDateTime()
        {
            try
            {
                var date = new DateTime(Year, Month, Day, Hour, Minute, 0);
                return date;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}