using System;
using System.Linq;

namespace Rogero.SchedulingLibrary
{
    public class Time : IEquatable<Time>
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
            return new Time(template.Minutes.First(), template.Hours.First(), template.DaysOfMonth.First(), month, Year);
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
        
        public static bool operator >(Time c1, Time c2)
        {
            return c1.ToDateTime() > c2.ToDateTime();
        }

        public static bool operator <(Time c1, Time c2)
        {
            return c1.ToDateTime() < c2.ToDateTime();
        }

        public Time ChangeYear(int year, CronTemplate template)
        {
            return new Time(template.Minutes.First(), template.Hours.First(), template.DaysOfMonth.First(), template.Months.First(), year);
        }

        public bool Equals(Time other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Minute == other.Minute && Hour == other.Hour && Day == other.Day && Month == other.Month && Year == other.Year;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Time)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Minute;
                hashCode = (hashCode * 397) ^ Hour;
                hashCode = (hashCode * 397) ^ Day;
                hashCode = (hashCode * 397) ^ Month;
                hashCode = (hashCode * 397) ^ Year;
                return hashCode;
            }
        }

        public static bool operator ==(Time left, Time right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Time left, Time right)
        {
            return !Equals(left, right);
        }
    }
}