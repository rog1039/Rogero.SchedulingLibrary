using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogero.SchedulingLibrary
{
    public class CronTimeIncrementor
    {
        public static CronTime Increment(CronTime cronTime)
        {
            var minuteResult = IncrementList(cronTime.Time.Minute, cronTime.CronTemplate.Minutes);
            if (minuteResult.NoOverflow)
                return new CronTime(cronTime.CronTemplate, cronTime.Time.ChangeMinute(minuteResult.Value, cronTime.CronTemplate));

            var hourResult = IncrementList(cronTime.Time.Hour, cronTime.CronTemplate.Hours);
            if (hourResult.NoOverflow)
                return new CronTime(cronTime.CronTemplate, cronTime.Time.ChangeHour(hourResult.Value, cronTime.CronTemplate));

            while (true)
            {
                var dayResult = IncrementList(cronTime.Time.Day, cronTime.CronTemplate.DaysOfMonth);

                if (dayResult.NoOverflow)
                    cronTime = new CronTime(cronTime.CronTemplate, cronTime.Time.ChangeDay(dayResult.Value, cronTime.CronTemplate));
                else
                    cronTime = IncrementMonth(cronTime);

                var date = cronTime.Time.ToDateTime();
                if (date == null)
                    continue;

                var matchDayOfWeek = MatchDayOfWeek(cronTime.CronTemplate, date.Value);
                if (matchDayOfWeek) return cronTime;
            }
        }

        private static IncrementListResult IncrementList(int currentValue, IList<int> possibleValues)
        {
            var nextIndex = possibleValues.IndexOf(currentValue) + 1;
            var highestIndex = possibleValues.Count - 1;
            var overflowed = nextIndex > highestIndex;

            return overflowed
                ? IncrementListResult.ValueWithOverflow(possibleValues[0])
                : IncrementListResult.ValueWithoutOverflow(possibleValues[nextIndex]);
        }
        
        private static CronTime IncrementMonth(CronTime cronTime)
        {
            var monthResult = IncrementList(cronTime.Time.Month, cronTime.CronTemplate.Months);
            var year = monthResult.NoOverflow ? cronTime.Time.Year : cronTime.Time.Year + 1;
            return new CronTime(
                cronTime.CronTemplate,
                new Time(
                    cronTime.CronTemplate.Minutes.First(),
                    cronTime.CronTemplate.Hours.First(),
                    cronTime.CronTemplate.DaysOfMonth.First(),
                    monthResult.Value,
                    year));
        }

        private static bool MatchDayOfWeek(CronTemplate cronTemplate, DateTime date)
        {
            var dayOfWeekMatch = cronTemplate.DayOfWeek.Contains((int)date.DayOfWeek);
            return dayOfWeekMatch;
        }
    }
}