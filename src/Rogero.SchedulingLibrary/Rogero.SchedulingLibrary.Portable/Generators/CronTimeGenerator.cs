using System;
using System.Collections.Generic;
using System.Linq;
using Rogero.SchedulingLibrary.Infrastructure;

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
                priorityList.AddRange(newCronTimes, cronTime => cronTime.Time);
                priorityList.Remove(first.Key);
                yield return first.Value.First();
            }
        }

        private static SortedDictionary<Time, IList<CronTime>> CreateCronTimePriorityList(DateTime dateTime, CronTemplate[] cronTemplates)
        {
            var cronTimes = cronTemplates.Select(template => GetNextCronTime(dateTime, template)).ToList();
            return cronTimes.ToSortedDictionaryMany(z => z.Time);
        }

        private static CronTime GetNextCronTime(DateTime dateTime, CronTemplate template)
        {
            var cronTime = new CronTime(template, dateTime);
            if (cronTime.DateTime.Value == dateTime)
                cronTime = cronTime.GetNext();
            return cronTime;
        }
    }
}