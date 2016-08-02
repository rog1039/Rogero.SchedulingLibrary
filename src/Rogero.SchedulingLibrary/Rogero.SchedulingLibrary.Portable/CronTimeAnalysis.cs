namespace Rogero.SchedulingLibrary
{
    public class CronTimeAnalysis
    {
        public bool SecondHasIndex { get; }
        public bool MinuteHasIndex { get; }
        public bool HourHasIndex { get; }
        public bool DayOfMonthHasIndex { get; }
        public bool MonthHasIndex { get; }
        public bool DayOfWeekHasIndex { get; }
        
        public CronTimeAnalysis(CronTime cronTime)
        {
            var time = cronTime.Time;
            var template = cronTime.CronTemplate;

            SecondHasIndex = template.Seconds.Contains(time.Second);
            MinuteHasIndex = template.Minutes.Contains(time.Minute);
            HourHasIndex = template.Hours.Contains(time.Hour);
            DayOfMonthHasIndex = template.DaysOfMonth.Contains(time.Day);
            MonthHasIndex = template.Months.Contains(time.Month);
            DayOfWeekHasIndex = cronTime.DateTime.HasValue && CronTime.MatchDayOfWeek(template, cronTime.DateTime.Value);
        }
    }
}