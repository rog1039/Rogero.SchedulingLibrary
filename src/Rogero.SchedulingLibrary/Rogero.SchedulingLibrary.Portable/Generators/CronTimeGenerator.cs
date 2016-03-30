using System;
using System.Collections.Generic;

namespace Rogero.SchedulingLibrary.Generators
{
    //public static class CronTimeGenerator
    //{
    //    public static IEnumerable<CronTime> Generate(CronTime cronTime)
    //    {
    //        while (true)
    //        {
    //            cronTime = cronTime.GetNext();
    //            yield return cronTime;
    //        }
    //    }

    //    public static IEnumerable<CronTime> GenerateFor(CronTime cronTime, TimeSpan timeSpan)
    //    {
    //        var cutoffTime = cronTime.Time.ToDateTime().Value + timeSpan;
    //        while (true)
    //        {
    //            cronTime = cronTime.GetNext();
    //            if (cronTime.Time.ToDateTime().Value > cutoffTime) yield break;
    //            yield return cronTime;
    //        }
    //    }

    //    public static IEnumerable<CronTime> GenerateForLessorOf(CronTime cronTime, TimeSpan timeSpan, int maxReturned)
    //    {
    //        var count = 0;
    //        foreach (var time in GenerateFor(cronTime, timeSpan))
    //        {
    //            count++;
    //            if (count > maxReturned) yield break;
    //            yield return time;
    //        }
    //    }

    //    public static IEnumerable<CronTime> CreateByLesserOfTimeSpanOrCountWithMinimum(CronTime time, TimeSpan timeSpan, int minCount, int maxCount)
    //    {
    //        var count = 0;
    //        foreach (var cronTime in Generate(time))
    //        {
    //            count++;
    //            if (count > maxCount) yield break;
    //            if (count > minCount && cronTime > time) yield break;
    //            yield return cronTime;
    //        }
    //    }
    //}
}