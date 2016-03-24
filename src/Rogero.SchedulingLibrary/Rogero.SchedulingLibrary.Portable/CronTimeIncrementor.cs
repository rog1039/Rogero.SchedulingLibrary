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

    public class CronTimeAnalysis
    {
        public bool MinuteHasIndex { get; }
        public bool HourHasIndex { get; }
        public bool DayOfMonthHasIndex { get; }
        public bool MonthHasIndex { get; }
        public bool DayOfWeekHasIndex { get; }

        public CronTimeAnalysis(bool minuteHasIndex, bool hourHasIndex, bool dayOfMonthHasIndex, bool monthHasIndex, bool dayOfWeekHasIndex)
        {
            MinuteHasIndex = minuteHasIndex;
            HourHasIndex = hourHasIndex;
            DayOfMonthHasIndex = dayOfMonthHasIndex;
            MonthHasIndex = monthHasIndex;
            DayOfWeekHasIndex = dayOfWeekHasIndex;
        }

        public CronTimeAnalysis(CronTime cronTime)
        {
            var time = cronTime.Time;
            var template = cronTime.CronTemplate;
            MinuteHasIndex = template.Minutes.Contains(time.Minute);
            HourHasIndex = template.Hours.Contains(time.Hour);
            DayOfMonthHasIndex = template.DaysOfMonth.Contains(time.Day);
            MonthHasIndex = template.Months.Contains(time.Month);
            DayOfWeekHasIndex = cronTime.DateTime.HasValue && CronTime.MatchDayOfWeek(template, cronTime.DateTime.Value);
        }
    }

    //public class CronTimeAnalysisResult
    //{
    //    public IntInListAnalysisResult MinuteAnalysis { get; }
    //    public IntInListAnalysisResult HourAnalysis { get; }
    //    public IntInListAnalysisResult DayOfMonthAnalysis { get; }
    //    public IntInListAnalysisResult MonthAnalysis { get; }
    //    public bool DayOfWeekIsValid { get; }

    //    public CronTimeAnalysisResult(CronTime cronTime)
    //    {
    //        var time = cronTime.Time;
    //        var template = cronTime.CronTemplate;
    //        MonthAnalysis = new IntInListAnalysisResult(time.Month, template.Months);
    //        DayOfMonthAnalysis = new IntInListAnalysisResult(time.Day, template.DaysOfMonth);
    //        HourAnalysis = new IntInListAnalysisResult(time.Hour, template.Hours);
    //        MinuteAnalysis = new IntInListAnalysisResult(time.Minute, template.Minutes);
    //        DayOfWeekIsValid = cronTime.DateTime.HasValue && CronTime.MatchDayOfWeek(template, cronTime.DateTime.Value);
    //    }
    //}

    //public class IntInListAnalysisResult
    //{
    //    public IntInListResultEnum IntInListResultEnum { get; }
    //    public int? NextClosestUpperIndex { get; }
    //    public int? Index { get; set; }

    //    public IntInListAnalysisResult(int value, IList<int> list)
    //    {
    //        if (value < list[0])
    //        {
    //            NextClosestUpperIndex = 0;
    //            IntInListResultEnum = IntInListResultEnum.Before;
    //        }
    //        else if (value == list[0])
    //        {
    //            Index = 0;
    //            IntInListResultEnum = IntInListResultEnum.First;
    //        }
    //        else if (value == list.Last())
    //        {
    //            Index = list.Count - 1;
    //            IntInListResultEnum = IntInListResultEnum.Last;
    //        }
    //        else if (value > list.Last())
    //        {
    //            IntInListResultEnum = IntInListResultEnum.After;
    //        }
    //        else
    //        {
    //            var state = IndexStateMachine.Started;
    //            for (int i = 0; i < list.Count; i++)
    //            {
    //                int v = list[i];
    //                if (value == v)
    //                {
    //                    Index = i;
    //                    IntInListResultEnum = IntInListResultEnum.MiddleIndex;
    //                    break;
    //                }
                    
    //                if (state == IndexStateMachine.Started || state == IndexStateMachine.LessThan)
    //                {
    //                    if (v < value)
    //                        state = IndexStateMachine.LessThan;
    //                    else
    //                    {
    //                        state = IndexStateMachine.GreaterThan;
    //                        NextClosestUpperIndex = i;
    //                        IntInListResultEnum = IntInListResultEnum.InBetween;
    //                    }
    //                }
    //            }
    //        }
    //    }

    //    private enum IndexStateMachine
    //    {
    //        Started,
    //        LessThan,
    //        Equal,
    //        GreaterThan
    //    }
    //}

    //public enum IntInListResultEnum
    //{
    //    Before,
    //    First, 
    //    InBetween,
    //    MiddleIndex,
    //    Last,
    //    After
    //}
}