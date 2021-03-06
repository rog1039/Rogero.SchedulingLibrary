using System;
using System.Collections.Generic;
using System.Linq;
// ReSharper disable PossibleMultipleEnumeration

namespace Rogero.SchedulingLibrary.Generators
{
    public static class CronTimeSeriesExtensionMethods
    {
        public static IEnumerable<CronTime> ForTime(this IEnumerable<CronTime> cronTimes, TimeSpan timeSpan)
        {
            var startingCronTime = cronTimes.First();
            var cutoffTime = startingCronTime.Time.ToDateTime().Value + timeSpan;
            foreach (var cronTime in cronTimes)
            {
                if (cronTime.DateTime.Value > cutoffTime) yield break;
                yield return cronTime;
            }
        }

        public static IEnumerable<CronTime> ForLesserOf(this IEnumerable<CronTime> cronTimes, TimeSpan timeSpan, int maxReturned)
        {
            return cronTimes.ForTime(timeSpan).Take(maxReturned);
        }

        public static IEnumerable<CronTime> ForLesserOfWithMinimum(this IEnumerable<CronTime> cronTimes, TimeSpan timeSpan, int count, int minReturned)
        {
            var iterationCount = 0;
            var cutoffTime = cronTimes.First().Time.ToDateTime().Value + timeSpan;
            foreach (var cronTime in cronTimes)
            {
                if (iterationCount < minReturned)
                {
                    yield return cronTime;
                }
                else
                {
                    if (iterationCount >= count) yield break;
                    if (cronTime.DateTime.Value > cutoffTime) yield break;
                    yield return cronTime;
                }
                iterationCount++;
            }
        }
    }
}