using System;
using System.Collections.Generic;
using System.Linq;
using Rogero.Option;

// ReSharper disable IteratorNeverReturns

namespace Rogero.SchedulingLibrary.Generators
{
    public static class CronTimeGenerator
    {
        public static IEnumerable<CronTime> Generate(DateTime dateTime, params CronTemplate[] cronTemplates)
        {
            var priorityList = CreateCronTimePriorityList(dateTime, cronTemplates);

            while (true)
            {
                var first = priorityList.First();
                var newCronTimes = first.Value.Select(z => z.GetNext()).ToList();
                AddCronTimesToList(priorityList, newCronTimes);
                priorityList.Remove(first.Key);
                yield return first.Value.First();
            }
        }

        private static SortedDictionary<Time, IList<CronTime>> CreateCronTimePriorityList(DateTime dateTime, CronTemplate[] cronTemplates)
        {
            var cronTimes = cronTemplates.Select(template => new CronTime(template, dateTime));
            var dictionary = new SortedDictionary<Time, IList<CronTime>>();
            foreach (var cronTime in cronTimes)
            {
                AddCronTimeToList(dictionary, cronTime);
            }
            return dictionary;
        }

        private static void AddCronTimesToList(SortedDictionary<Time, IList<CronTime>> priorityList, List<CronTime> cronTimes)
        {
            foreach (var cronTime in cronTimes)
            {
                AddCronTimeToList(priorityList, cronTime);
            }
        }

        private static void AddCronTimeToList(SortedDictionary<Time, IList<CronTime>> priorityList, CronTime cronTime)
        {
            var group = priorityList.TryGetValue(cronTime.Time);
            if (group.HasValue)
            {
                group.Value.Add(cronTime);
            }
            else
            {
                priorityList.Add(cronTime.Time, new List<CronTime>() {cronTime});
            }
        }
    }
}