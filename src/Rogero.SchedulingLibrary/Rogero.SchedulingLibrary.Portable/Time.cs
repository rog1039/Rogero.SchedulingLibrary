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

        public Time(int year, int month, int day, int hour, int minute)
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
            return new Time(Year, Month, Day, Hour, minute);
        }

        public Time ChangeHour(int hour, CronTemplate template)
        {
            return new Time(Year, Month, Day, hour, template.Minutes.First());
        }

        public Time ChangeDay(int day, CronTemplate template)
        {
            return new Time(Year, Month, day, template.Hours.First(), template.Minutes.First());
        }

        public Time ChangeMonth(int month, CronTemplate template)
        {
            return new Time(Year, month, template.DaysOfMonth.First(), template.Hours.First(), template.Minutes.First());
        }

        public Time ChangeYear(int year, CronTemplate template)
        {
            return new Time(year, template.Months.First(), template.DaysOfMonth.First(), template.Hours.First(), template.Minutes.First());
        }

        public DateTime? ToDateTime()
        {
            try
            {
                var date = new DateTime(Year, Month, Day, Hour, Minute, 0);
                return date;
            }
            catch (Exception)
            {
                return null;
            }
        }
        
    }
}