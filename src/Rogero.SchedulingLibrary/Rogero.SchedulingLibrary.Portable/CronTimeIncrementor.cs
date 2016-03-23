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
            return conversionResult.CronTime.IncrementMinute();
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