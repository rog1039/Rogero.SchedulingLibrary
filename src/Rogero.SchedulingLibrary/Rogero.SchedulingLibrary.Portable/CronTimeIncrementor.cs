using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogero.SchedulingLibrary
{
    public class CronTimeIncrementor
    {
        public static CronTime Increment(CronTime cronTime)
        {
            var conversionResult = ToValidCronTime(cronTime);
            if (!conversionResult.AlreadyValid) return conversionResult.CronTime;
            //return conversionResult.CronTime.IncrementMinute();

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
                    cronTime = cronTime.IncrementMonth();

                var date = cronTime.Time.ToDateTime();
                if (date == null)
                    continue;

                var matchDayOfWeek = MatchDayOfWeek(cronTime.CronTemplate, date.Value);
                if (matchDayOfWeek) return cronTime;
            }
        }

        public static CronTimeToValidCronTimeConversionResult ToValidCronTime(CronTime cronTime)
        {
            var analysis = new CronTimeAnalysisResult(cronTime);
            var time = cronTime.Time;
            var template = cronTime.CronTemplate;

            //Check month
            if (analysis.MonthAnalysis.IntInListResultEnum == IntInListResultEnum.Before)
            {
                return CronTimeToValidCronTimeConversionResult.NotValid(cronTime.ChangeMonth(template.Months[0]));
            }
            else if (analysis.MonthAnalysis.IntInListResultEnum == IntInListResultEnum.InBetween)
            {
                return CronTimeToValidCronTimeConversionResult.NotValid(cronTime.ChangeMonth(template.Months[analysis.MonthAnalysis.NextClosestUpperIndex.Value]));
            }
            else if (analysis.MonthAnalysis.IntInListResultEnum == IntInListResultEnum.After)
            {
                return CronTimeToValidCronTimeConversionResult.NotValid(cronTime.ChangeYear(time.Year+1));
            }

            //So the month is valid....now onto day
            if (analysis.DayAnalysis.IntInListResultEnum == IntInListResultEnum.Before)
            {
                return CronTimeToValidCronTimeConversionResult.NotValid(cronTime.ChangeDay(template.DaysOfMonth[0]));
            }
            else if (analysis.DayAnalysis.IntInListResultEnum == IntInListResultEnum.InBetween)
            {
                return CronTimeToValidCronTimeConversionResult.NotValid(cronTime.ChangeDay(template.DaysOfMonth[analysis.DayAnalysis.NextClosestUpperIndex.Value]));
            }
            else if (analysis.DayAnalysis.IntInListResultEnum == IntInListResultEnum.After)
            {
                return CronTimeToValidCronTimeConversionResult.NotValid(cronTime.IncrementMonth());
            }

            //So the day is valid....now onto hour
            if (analysis.HourAnalysis.IntInListResultEnum == IntInListResultEnum.Before)
            {
                return CronTimeToValidCronTimeConversionResult.NotValid(cronTime.ChangeHour(template.Hours[0]));
            }
            else if (analysis.HourAnalysis.IntInListResultEnum == IntInListResultEnum.InBetween)
            {
                return CronTimeToValidCronTimeConversionResult.NotValid(cronTime.ChangeHour(template.Hours[analysis.HourAnalysis.NextClosestUpperIndex.Value]));
            }
            else if (analysis.HourAnalysis.IntInListResultEnum == IntInListResultEnum.After)
            {
                return CronTimeToValidCronTimeConversionResult.NotValid(cronTime.IncrementDay());
            }

            //So the hour is valid....now onto minute
            if (analysis.MinuteAnalysis.IntInListResultEnum == IntInListResultEnum.Before)
            {
                return CronTimeToValidCronTimeConversionResult.NotValid(cronTime.ChangeMinute(template.Minutes[0]));
            }
            else if (analysis.MinuteAnalysis.IntInListResultEnum == IntInListResultEnum.InBetween)
            {
                return CronTimeToValidCronTimeConversionResult.NotValid(cronTime.ChangeMinute(template.Minutes[analysis.MinuteAnalysis.NextClosestUpperIndex.Value]));
            }
            else if (analysis.MinuteAnalysis.IntInListResultEnum == IntInListResultEnum.After)
            {
                return CronTimeToValidCronTimeConversionResult.NotValid(cronTime.IncrementHour());
            }

            return CronTimeToValidCronTimeConversionResult.Valid(cronTime);
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

    public class CronTimeToValidCronTimeConversionResult
    {
        public bool AlreadyValid { get; }
        public CronTime CronTime { get; }

        private CronTimeToValidCronTimeConversionResult(bool alreadyValid, CronTime cronTime)
        {
            AlreadyValid = alreadyValid;
            CronTime = cronTime;
        }

        public static CronTimeToValidCronTimeConversionResult Valid(CronTime cronTime)
        {
            return new CronTimeToValidCronTimeConversionResult(true, cronTime);
        }

        public static CronTimeToValidCronTimeConversionResult NotValid(CronTime cronTime)
        {
            return new CronTimeToValidCronTimeConversionResult(false, cronTime);
        }
    }

    public class CronTimeAnalysisResult
    {
        public IntInListAnalysisResult MinuteAnalysis { get; }
        public IntInListAnalysisResult HourAnalysis { get; }
        public IntInListAnalysisResult DayAnalysis { get; }
        public IntInListAnalysisResult MonthAnalysis { get; }
        
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
}