using System;
using System.Collections.Generic;
using System.Linq;
using Rogero.Option;

namespace Rogero.SchedulingLibrary
{
    public class CronTimeIncrementor
    {
        public static CronTime Increment(CronTime cronTime)
        {
            var newValidCronTime = GetValidCronTimeIfNotValid(cronTime);
            return newValidCronTime.HasValue
                ? newValidCronTime.Value
                : cronTime.IncrementMinute();
        }

        public static Option<CronTime> GetValidCronTimeIfNotValid(CronTime cronTime)
        {
            var cronTimeAnalysis = new CronTimeAnalysis(cronTime);

            if (!cronTimeAnalysis.MonthHasIndex)
                return cronTime.IncrementMonth();

            if (!cronTimeAnalysis.DayOfMonthHasIndex || !cronTimeAnalysis.DayOfWeekHasIndex)
                return cronTime.IncrementDay();

            if (!cronTimeAnalysis.HourHasIndex)
                return cronTime.IncrementHour();

            if (!cronTimeAnalysis.MinuteHasIndex)
                return cronTime.IncrementMinute();

            return Option<CronTime>.None;
        }
    }
}