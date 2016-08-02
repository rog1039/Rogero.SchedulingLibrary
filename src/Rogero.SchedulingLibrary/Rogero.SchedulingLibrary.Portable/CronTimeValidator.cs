using Rogero.Options;

namespace Rogero.SchedulingLibrary
{
    public class CronTimeValidator
    {
        public static Option<CronTime> GetNextCronTimeThatFitsTheTemplate(CronTime cronTime)
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

            if (!cronTimeAnalysis.SecondHasIndex)
                return cronTime.IncrementSecond();

            return Option<CronTime>.None;
        }
    }
}