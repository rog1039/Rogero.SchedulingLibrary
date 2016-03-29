using System;
using System.Collections.Generic;

namespace Rogero.SchedulingLibrary
{
    public partial class CronTime : IEquatable<CronTime>, IComparable<CronTime>, IComparable
    {
        public CronTemplate CronTemplate { get; }
        public Time Time { get; }
        public DateTime? DateTime => Time.ToDateTime();

        public CronTime(CronTemplate cronTemplate, Time time, bool forceToValidCronTime = true)
        {
            CronTemplate = cronTemplate;
            Time = time;

            if (forceToValidCronTime)
            {
                var validCronTime = CronTimeValidator.GetNextCronTimeThatFitsTheTemplate(this);
                if (validCronTime.HasValue) Time = validCronTime.Value.Time;
            }
        }

        public CronTime(CronTemplate cronTemplate, DateTime datetime, bool forceToValidCronTime = true)
        {
            CronTemplate = cronTemplate;
            Time = new Time(datetime);
            if (forceToValidCronTime)
            {
                var validCronTime = CronTimeValidator.GetNextCronTimeThatFitsTheTemplate(this);
                if (validCronTime.HasValue) Time = validCronTime.Value.Time;
            }
        }

        public CronTime AdvanceTo(DateTime dateTime)
        {
            var cronTime = new CronTime(CronTemplate, dateTime);
            return cronTime;
        }
        
        public CronTime GetNext()
        {
            var cronTime = this;
            while (true)
            {
                cronTime = cronTime.IncrementMinute();
                var dateTime = cronTime.Time.ToDateTime();
                if (dateTime.HasValue) return cronTime;
            }
        }

        internal CronTime ChangeYear(int year)
        {
            return new CronTime(CronTemplate, Time.ChangeYear(year, CronTemplate));
        }

        internal CronTime ChangeMonth(int month)
        {
            return new CronTime(CronTemplate, Time.ChangeMonth(month, CronTemplate));
        }

        internal CronTime ChangeDay(int day)
        {
            return new CronTime(CronTemplate, Time.ChangeDay(day, CronTemplate));
        }

        internal CronTime ChangeHour(int hour)
        {
            return new CronTime(CronTemplate, Time.ChangeHour(hour, CronTemplate));
        }

        internal CronTime ChangeMinute(int minute)
        {
            return new CronTime(CronTemplate, Time.ChangeMinute(minute, CronTemplate));
        }


        internal CronTime IncrementMonth()
        {
            var monthResult = IncrementList(Time.Month, CronTemplate.Months);
            return monthResult.Overflow
                ? ChangeYear(Time.Year + 1)
                : ChangeMonth(monthResult.Value);
        }

        internal CronTime IncrementDay()
        {
            var cronTime = this;
            while (true)
            {
                var dayResult = IncrementList(cronTime.Time.Day, cronTime.CronTemplate.DaysOfMonth);
                cronTime = dayResult.Overflow
                    ? IncrementMonth()
                    : ChangeDay(dayResult.Value);
                if (cronTime.DateTime.HasValue && MatchDayOfWeek(CronTemplate, cronTime.DateTime.Value)) return cronTime;
            }
        }

        internal static bool MatchDayOfWeek(CronTemplate cronTemplate, DateTime date)
        {
            var dayOfWeekMatch = cronTemplate.DayOfWeek.Contains((int) date.DayOfWeek);
            return dayOfWeekMatch;
        }

        internal CronTime IncrementHour()
        {
            var hourResult = IncrementList(Time.Hour, CronTemplate.Hours);
            return hourResult.Overflow
                ? IncrementDay()
                : ChangeHour(hourResult.Value);
        }

        internal CronTime IncrementMinute()
        {
            var minuteResult = IncrementList(Time.Minute, CronTemplate.Minutes);
            return minuteResult.Overflow
                ? IncrementHour()
                : ChangeMinute(minuteResult.Value);
        }

        private static IncrementListResult IncrementList(int currentValue, IList<int> possibleValues)
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