using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogero.SchedulingLibrary
{
    public static class CronTimeGenerator
    {
        public static IEnumerable<CronTime> Generate(CronTime cronTime)
        {
            while (true)
            {
                cronTime = cronTime.GetNext();
                yield return cronTime;
            }
        }

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

        public static IEnumerable<CronTime> ForLesserOfWithMinimum(this IEnumerable<CronTime> cronTimes, TimeSpan timeSpan, int maxReturned, int minReturned)
        {
            var count = 0;
            //Provide at minimum number....
            foreach (var cronTime in cronTimes)
            {
                if (count >= minReturned) break;
                yield return cronTime;
                count++;
            }

            var cutoffTime = cronTimes.First().Time.ToDateTime().Value + timeSpan;
            foreach (var cronTime in cronTimes)
            {
                if (count >= maxReturned) yield break;
                if (cronTime.DateTime.Value > cutoffTime) yield break;
                yield return cronTime;
                count++;
            }
        }

        public static IEnumerable<CronTime> GenerateFor(CronTime cronTime, TimeSpan timeSpan)
        {
            var cutoffTime = cronTime.Time.ToDateTime().Value + timeSpan;
            while (true)
            {
                cronTime = cronTime.GetNext();
                if (cronTime.Time.ToDateTime().Value > cutoffTime) yield break;
                yield return cronTime;
            }
        }

        public static IEnumerable<CronTime> GenerateForLessorOf(CronTime cronTime, TimeSpan timeSpan, int maxReturned)
        {
            var count = 0;
            foreach (var time in GenerateFor(cronTime, timeSpan))
            {
                count++;
                if (count > maxReturned) yield break;
                yield return time;
            }
        }

        public static IEnumerable<CronTime> CreateByLesserOfTimeSpanOrCountWithMinimum(CronTime time, TimeSpan timeSpan, int minCount, int maxCount)
        {
            var count = 0;
            foreach (var cronTime in Generate(time))
            {
                count++;
                if (count > maxCount) yield break;
                if (count > minCount && cronTime > time) yield break;
                yield return cronTime;
            }
        }
    }

    public static class CronTimeAggregateGenerator
    {
        public static IEnumerable<CronTime> Generate(DateTime dateTime, params CronTemplate[] cronTemplates)
        {
            var cronTimePriorityList = CreateCronTimePriorityQueue(dateTime, cronTemplates);

            while (true)
            {
                var firstEntry = cronTimePriorityList.First();
                var newEntry = firstEntry.GetNext();
                cronTimePriorityList.Add(newEntry);
                cronTimePriorityList.Remove(firstEntry);
                yield return firstEntry;
            }
        }

        private static SortedSet<CronTime> CreateCronTimePriorityQueue(DateTime dateTime, CronTemplate[] cronTemplates)
        {
            var cronTimes = cronTemplates.Select(template => new CronTime(template, dateTime));
            SortedSet<CronTime> priorityQueue = new SortedSet<CronTime>(cronTimes);
            return priorityQueue;
        }
    }
}