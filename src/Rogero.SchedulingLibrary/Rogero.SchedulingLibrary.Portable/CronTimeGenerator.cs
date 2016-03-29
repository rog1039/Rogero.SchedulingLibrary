using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogero.SchedulingLibrary
{
    public class CronTimeGenerator
    {
        public static IEnumerable<CronTime> Generate(CronTime cronTime)
        {
            while (true)
            {
                cronTime = cronTime.GetNext();
                yield return cronTime;
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

    public class CronTimeAggregateGenerator
    {
        public static IEnumerable<CronTime> Generate(DateTime dateTime, params CronTemplate[] cronTemplates)
        {
            var cronTimePriorityList = CreateCronTimePriorityQueue(dateTime, cronTemplates);

            while (true)
            {
                var firstEntry = cronTimePriorityList.First();
                var newEntry = firstEntry.Key.GetNext();
                cronTimePriorityList.Add(newEntry, firstEntry.Value);
                yield return firstEntry.Key;
            }
        }

        private static SortedDictionary<CronTime, int> CreateCronTimePriorityQueue(DateTime dateTime, CronTemplate[] cronTemplates)
        {
            var cronTimes = cronTemplates.Select(template => new CronTime(template, dateTime));
            SortedDictionary<CronTime, int> priorityQueue = new SortedDictionary<CronTime, int>();
            
            //Seed priority list
            var initialEntries = cronTimes
                .Select(((cronTime, index) => new KeyValuePair<CronTime, int>(cronTime, index)))
                .ToList();
            foreach (var initialEntry in initialEntries)
            {
                priorityQueue.Add(initialEntry.Key, initialEntry.Value);
            }
            return priorityQueue;
        }

        public static IEnumerable<CronTime> Generate2(DateTime dateTime, params CronTemplate[] cronTemplates)
        {
            var cronTimePriorityList = CreateCronTimePriorityQueue2(dateTime, cronTemplates);

            while (true)
            {
                var firstEntry = cronTimePriorityList.First();
                var newEntry = firstEntry.GetNext();
                cronTimePriorityList.Add(newEntry);
                cronTimePriorityList.Remove(firstEntry);
                yield return firstEntry;
            }
        }

        private static SortedSet<CronTime> CreateCronTimePriorityQueue2(DateTime dateTime, CronTemplate[] cronTemplates)
        {
            var cronTimes = cronTemplates.Select(template => new CronTime(template, dateTime));
            SortedSet<CronTime> priorityQueue = new SortedSet<CronTime>(cronTimes);
            return priorityQueue;
        }
    }
}