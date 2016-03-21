using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogero.SchedulingLibrary
{
    public class CronTimeIncrementor
    {
        public static CronTime Increment(CronTime cronTime)
        {
            //cronTime = ConvertIncomingCronTimeToValidCronTime(cronTime);
            var minuteResult = IncrementList(cronTime.Time.Minute, cronTime.CronTemplate.Minutes);
            if (minuteResult.NoOverflow)
                return cronTime.ChangeMinute(minuteResult.Value);

            var hourResult = IncrementList(cronTime.Time.Hour, cronTime.CronTemplate.Hours);
            if (hourResult.NoOverflow)
                return cronTime.ChangeHour(hourResult.Value);

            while (true)
            {
                var dayResult = IncrementList(cronTime.Time.Day, cronTime.CronTemplate.DaysOfMonth);

                if (dayResult.NoOverflow)
                    cronTime = cronTime.ChangeDay(dayResult.Value);
                else
                    cronTime = IncrementMonth(cronTime);

                var date = cronTime.Time.ToDateTime();
                if (date == null)
                    continue;

                var matchDayOfWeek = MatchDayOfWeek(cronTime.CronTemplate, date.Value);
                if (matchDayOfWeek) return cronTime;
            }
        }

        public static CronTime ConvertIncomingCronTimeToValidCronTime(CronTime cronTime)
        {
            var template = cronTime.CronTemplate;
            var time = cronTime.Time;

            //var analysisResult = AnalyzeCronTime(cronTime);
            //if (analysisResult.MonthAnalysis == IntInListAnalysisResult.After)
            //    return new CronTime(template, time.ChangeYear(time.Year + 1, template));

            return null;

            //if (time.Month > template.Months.Last())
            //    return new CronTime(template, time.ChangeYear(time.Year + 1, template));
            //if (time.Day > template.DaysOfMonth.Last())
            //{
            //    var dayResult = IncrementList(cronTime.Time.Day, cronTime.CronTemplate.DaysOfMonth);

            //    return new CronTime(template, time.ChangeMonth(template.DaysOfMonth.First(), template));
            //}





            //if (time.Month > template.Months.Last())
            //    return new CronTime(template, time.ChangeYear(time.Year+1, template));
            //if (time.Day > template.DaysOfMonth.Last())
            //    return new CronTime(template, time.ChangeMonth(template.DaysOfMonth.First(), template));
            //if(time.Hour > template.Hours.Last())
            //    return new CronTime(template, time.ChangeHour(template.Hours.First(), template));
            //if(time.Minute > template.Minutes.Last())
            //    return new CronTime(template, time.ChangeMinute(template.Minutes.First(), template));

            //var dayResult = IncrementList(time.Day, template.DaysOfMonth);
            //if (dayResult.Overflow) return new CronTime(template, time.ChangeDay(dayResult.Value, template));

            //var hourResult = IncrementList(time.Hour, template.Hours);
            //if(hourResult.Overflow) return 

            //var minuteResult = IncrementList(time.Minute, template.Minutes);
            //var hourResult = IncrementList(time.Hour+overflowToInt(minuteResult.Overflow), template.Hours);

            ////minute = template.Minutes.ClosestLowerIndexOf(time.Minute) < 0 ? template.Minutes.First() : time.Minute;
            //minute = minuteResult.Value;
            //hour = template.Hours.IndexOf(time.Hour) < 0 ? template.Hours.First() : time.Hour;
            //day = template.DaysOfMonth.IndexOf(time.Day) < 0 ? template.DaysOfMonth.First() : time.Day;
            //month = template.Months.IndexOf(time.Month) < 0 ? template.Months.First() : time.Month;

            //return new CronTime(template, new Time(minute, hour, day, month, time.Year));
        }

        //private static CronTimeAnalysisResult AnalyzeCronTime(CronTime cronTime)
        //{
        //    var time = cronTime.Time;
        //    var template = cronTime.CronTemplate;

        //    var minuteResult = AnalyzeIntInLst(time.Minute, template.Minutes);
        //    var hourResult = AnalyzeIntInLst(time.Hour, template.Hours);
        //    var dayResult = AnalyzeIntInLst(time.Day, template.DaysOfMonth);
        //    var monthResult = AnalyzeIntInLst(time.Month, template.Months);

        //    var result = new CronTimeAnalysisResult(minuteResult, hourResult, dayResult, monthResult);
        //    return null;
        //}

        //private static IntInListAnalysisResult AnalyzeIntInLst(int number, IList<int> list)
        //{
        //    if(number < list.First())return IntInListAnalysisResult.Before;
        //    if(number > list.Last()) return IntInListAnalysisResult.After;
        //    if (number == list.First()) return IntInListAnalysisResult.First;
        //    if (number == list.Last()) return IntInListAnalysisResult.Last;
        //    return IntInListAnalysisResult.Middle;
        //}

        public static CronTime ToValidCronTime(CronTime cronTime)
        {
            var analysis = new CronTimeAnalysisResult(cronTime);
            var time = cronTime.Time;
            var template = cronTime.CronTemplate;

            if (analysis.MonthAnalysis.IntInListResultEnum == IntInListResultEnum.Before)
            {
                return cronTime.ChangeMonth(template.Months[0]);
            }
            else if (analysis.MonthAnalysis.IntInListResultEnum == IntInListResultEnum.InBetween)
            {
                return cronTime.ChangeMonth(template.Months[analysis.MonthAnalysis.NextClosestUpperIndex.Value]);
            }
            else if (analysis.MonthAnalysis.IntInListResultEnum == IntInListResultEnum.After)
            {
                return cronTime.ChangeYear(time.Year+1);
            }

            //So the month is valid....now onto day
            if (analysis.DayAnalysis.IntInListResultEnum == IntInListResultEnum.Before)
            {
                return cronTime.ChangeDay(template.DaysOfMonth[0]);
            }
            else if (analysis.DayAnalysis.IntInListResultEnum == IntInListResultEnum.InBetween)
            {
                return cronTime.ChangeDay(template.DaysOfMonth[analysis.DayAnalysis.NextClosestUpperIndex.Value]);
            }
            else if (analysis.DayAnalysis.IntInListResultEnum == IntInListResultEnum.After)
            {
                return cronTime.IncrementMonth();
            }

            //So the day is valid....now onto hour
            if (analysis.HourAnalysis.IntInListResultEnum == IntInListResultEnum.Before)
            {
                return cronTime.ChangeHour(template.Hours[0]);
            }
            else if (analysis.HourAnalysis.IntInListResultEnum == IntInListResultEnum.InBetween)
            {
                return cronTime.ChangeHour(template.Hours[analysis.HourAnalysis.NextClosestUpperIndex.Value]);
            }
            else if (analysis.HourAnalysis.IntInListResultEnum == IntInListResultEnum.After)
            {
                return cronTime.IncrementDay();
            }

            //So the hour is valid....now onto minute
            if (analysis.MinuteAnalysis.IntInListResultEnum == IntInListResultEnum.Before)
            {
                return cronTime.ChangeMinute(template.Hours[0]);
            }
            else if (analysis.MinuteAnalysis.IntInListResultEnum == IntInListResultEnum.InBetween)
            {
                return cronTime.ChangeMinute(template.Hours[analysis.MinuteAnalysis.NextClosestUpperIndex.Value]);
            }
            else if (analysis.MinuteAnalysis.IntInListResultEnum == IntInListResultEnum.After)
            {
                return cronTime.IncrementHour();
            }

            return cronTime;
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

    public class CronTimeAnalysisResult
    {
        public IntInListAnalysisResult MinuteAnalysis { get; }
        public IntInListAnalysisResult HourAnalysis { get; }
        public IntInListAnalysisResult DayAnalysis { get; }
        public IntInListAnalysisResult MonthAnalysis { get; }

        public CronTimeAnalysisResult(IntInListAnalysisResult minuteAnalysis, IntInListAnalysisResult hourAnalysis, IntInListAnalysisResult dayAnalysis, IntInListAnalysisResult monthAnalysis)
        {
            MinuteAnalysis = minuteAnalysis;
            HourAnalysis = hourAnalysis;
            DayAnalysis = dayAnalysis;
            MonthAnalysis = monthAnalysis;
        }

        public CronTimeAnalysisResult(CronTime cronTime)
        {
            var time = cronTime.Time;
            var template = cronTime.CronTemplate;
            MonthAnalysis = new IntInListAnalysisResult(time.Month, template.Months);
            DayAnalysis = new IntInListAnalysisResult(time.Day, template.DaysOfMonth);
            HourAnalysis = new IntInListAnalysisResult(time.Hour, template.Hours);
            MinuteAnalysis = new IntInListAnalysisResult(time.Minute, template.Minutes);
        }
    }

    public class IntInListAnalysisResult
    {
        public IntInListResultEnum IntInListResultEnum { get; }
        public int? NextClosestUpperIndex { get; }
        public int? Index { get; set; }

        public IntInListAnalysisResult(int value, IList<int> list)
        {
            if (value < list[0])
            {
                NextClosestUpperIndex = 0;
                IntInListResultEnum = IntInListResultEnum.Before;
            }
            else if (value == list[0])
            {
                Index = 0;
                IntInListResultEnum = IntInListResultEnum.First;
            }
            else if (value == list.Last())
            {
                Index = list.Count - 1;
                IntInListResultEnum = IntInListResultEnum.Last;
            }
            else if (value > list.Last())
            {
                IntInListResultEnum = IntInListResultEnum.After;
            }
            else
            {
                var state = IndexStateMachine.Started;
                for (int i = 0; i < list.Count; i++)
                {
                    int v = list[i];
                    if (value == v)
                    {
                        Index = i;
                        IntInListResultEnum = IntInListResultEnum.MiddleIndex;
                        break;
                    }
                    
                    if (state == IndexStateMachine.Started || state == IndexStateMachine.LessThan)
                    {
                        if (v < value)
                            state = IndexStateMachine.LessThan;
                        else
                        {
                            state = IndexStateMachine.GreaterThan;
                            NextClosestUpperIndex = i;
                            IntInListResultEnum = IntInListResultEnum.InBetween;
                        }
                    }
                }
            }
        }

        private enum IndexStateMachine
        {
            Started,
            LessThan,
            Equal,
            GreaterThan
        }
    }

    public enum IntInListResultEnum
    {
        Before,
        First, 
        InBetween,
        MiddleIndex,
        Last,
        After
    }

    public static class ListExtensionMethods
    {
        public static int ClosestLowerIndexOf(this IList<int> possibleValues, int value)
        {
            bool found = false;
            for (int i = 0; i < possibleValues.Count; i++)
            {
                if (value < possibleValues[i]) return i - 1;
            }
            return possibleValues.Count - 1;
        }
    }
    
}