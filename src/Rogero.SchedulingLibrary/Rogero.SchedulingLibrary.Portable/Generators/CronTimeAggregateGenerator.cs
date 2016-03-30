using System;
using System.Collections.Generic;
using System.Linq;
// ReSharper disable IteratorNeverReturns

namespace Rogero.SchedulingLibrary.Generators
{
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