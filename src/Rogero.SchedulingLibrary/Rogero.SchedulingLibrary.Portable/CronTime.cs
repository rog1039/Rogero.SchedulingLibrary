using System;
using System.Collections.Generic;

namespace Rogero.SchedulingLibrary
{
    public partial class CronTime : IEquatable<CronTime>, IComparable<CronTime>, IComparable
    {
        public CronTemplate CronTemplate { get; }
        public Time Time { get; }
        public DateTime DateTime => Time.ToDateTime().Value;

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

        private CronTime GetNextUnsafe()
        {
            return CronTimeIncrementor.Increment(this);
        }

        public CronTime GetNextEnsureValidDateTime()
        {
            var cronTime = this;
            while (true)
            {
                cronTime = cronTime.GetNextUnsafe();
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

        public CronTime IncrementMonth()
        {
            var monthResult = IncrementList(Time.Month, CronTemplate.Months);
            return monthResult.Overflow
                ? ChangeYear(Time.Year + 1)
                : ChangeMonth(monthResult.Value);
        }

        public CronTime IncrementDay()
        {

            var dayResult = IncrementList(Time.Day, CronTemplate.DaysOfMonth);
            return dayResult.Overflow
                ? IncrementMonth()
                : ChangeDay(dayResult.Value);
        }

        private static bool MatchDayOfWeek(CronTemplate cronTemplate, DateTime date)
        {
            var dayOfWeekMatch = cronTemplate.DayOfWeek.Contains((int)date.DayOfWeek);
            return dayOfWeekMatch;
        }

        public CronTime IncrementHour()
        {
            var hourResult = IncrementList(Time.Hour, CronTemplate.Hours);
            return hourResult.Overflow
                ? IncrementDay()
                : ChangeHour(hourResult.Value);
        }

        public CronTime IncrementMinute()
        {
            var minuteResult = IncrementList(Time.Minute, CronTemplate.Minutes);
            return minuteResult.Overflow
                ? IncrementHour()
                : ChangeMinute(minuteResult.Value);
        }

        public static IncrementListResult IncrementList(int currentValue, IList<int> possibleValues)
        {
            var nextIndex = possibleValues.ClosestLowerIndexOf(currentValue) + 1;
            var highestIndex = possibleValues.Count - 1;
            var overflowed = nextIndex > highestIndex;

            return overflowed
                ? IncrementListResult.ValueWithOverflow(possibleValues[0])
                : IncrementListResult.ValueWithoutOverflow(possibleValues[nextIndex]);
        }
    }
}