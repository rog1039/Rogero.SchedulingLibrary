using System;

namespace Rogero.SchedulingLibrary
{
    public class CronTime : IEquatable<CronTime>
    {
        public CronTemplate CronTemplate { get; }
        public Time Time { get; }

        public CronTime(CronTemplate cronTemplate, Time time)
        {
            CronTemplate = cronTemplate;
            Time = time;
        }

        public CronTime(CronTemplate cronTemplate, DateTime datetime)
        {
            CronTemplate = cronTemplate;
            Time = new Time(datetime);
        }

        public CronTime GetNext()
        {
            return CronTimeIncrementor.Increment(this);
        }

        public CronTime GetNextEnsureValidDateTime()
        {
            var cronTime = this;
            while (true)
            {
                cronTime = cronTime.GetNext();
                var dateTime = cronTime.Time.ToDateTime();
                if (dateTime.HasValue) return cronTime;
            }
        }

        public CronTime ChangeYear(int year)
        {
            return new CronTime(CronTemplate, Time.ChangeYear(year, CronTemplate));
        }

        public CronTime ChangeMonth(int month)
        {
            return new CronTime(CronTemplate, Time.ChangeMonth(month, CronTemplate));
        }

        public CronTime ChangeDay(int day)
        {
            return new CronTime(CronTemplate, Time.ChangeDay(day, CronTemplate));
        }

        public CronTime ChangeHour(int hour)
        {
            return new CronTime(CronTemplate, Time.ChangeHour(hour, CronTemplate));
        }

        public CronTime ChangeMinute(int minute)
        {
            return new CronTime(CronTemplate, Time.ChangeMinute(minute, CronTemplate));
        }


        public static bool operator >(CronTime c1, CronTime c2)
        {
            return c1.Time > c2.Time;
        }

        public static bool operator <(CronTime c1, CronTime c2)
        {
            return c1.Time < c2.Time;
        }

        public CronTime IncrementMonth()
        {
            var monthResult = CronTimeIncrementor.IncrementList(Time.Month, CronTemplate.Months);
            if (monthResult.Overflow)
            {
                return ChangeYear(Time.Year + 1);
            }
            else
            {
                return ChangeMonth(monthResult.Value);
            }
        }

        public CronTime IncrementDay()
        {
            var dayResult = CronTimeIncrementor.IncrementList(Time.Day, CronTemplate.DaysOfMonth);
            if (dayResult.Overflow)
            {
                return IncrementMonth();
            }
            else
            {
                return ChangeDay(dayResult.Value);
            }
        }

        public CronTime IncrementHour()
        {
            var hourResult = CronTimeIncrementor.IncrementList(Time.Hour, CronTemplate.Hours);
            if (hourResult.Overflow)
            {
                return IncrementDay();
            }
            else
            {
                return ChangeHour(hourResult.Value);
            }
        }

        public CronTime IncrementMinute()
        {
            throw new NotImplementedException();
        }

        public bool Equals(CronTime other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(CronTemplate, other.CronTemplate) && Equals(Time, other.Time);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CronTime) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((CronTemplate != null ? CronTemplate.GetHashCode() : 0)*397) ^ (Time != null ? Time.GetHashCode() : 0);
            }
        }

        public static bool operator ==(CronTime left, CronTime right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CronTime left, CronTime right)
        {
            return !Equals(left, right);
        }
    }
}