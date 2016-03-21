using System;
using System.Collections.Generic;

namespace Rogero.SchedulingLibrary
{
    public class CronTimeGenerator
    {
        public static IEnumerable<CronTime> Generate(CronTime cronTime)
        {
            while (true)
            {
                cronTime = cronTime.GetNextEnsureValidDateTime();
                yield return cronTime;
            }
        }

        public static IEnumerable<CronTime> GenerateFor(CronTime cronTime, TimeSpan timeSpan)
        {
            var cutoffTime = cronTime.Time.ToDateTime().Value + timeSpan;
            while (true)
            {
                cronTime = cronTime.GetNextEnsureValidDateTime();
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

    public class GenerationRequest
    {
        public static GenerationRequest CreateByCount(CronTime time, int count)
        {
            return new GenerationRequest(time, null, null, count);
        }

        public static GenerationRequest CreateByTimeSpan(CronTime time, TimeSpan timeSpan)
        {
            return new GenerationRequest(time, timeSpan, null, null);
        }

        public static GenerationRequest CreateByLesserOfTimeSpanOrCount(CronTime time, TimeSpan timeSpan, int maxCount)
        {
            return new GenerationRequest(time, timeSpan, null, maxCount);
        }

        public static GenerationRequest CreateByLesserOfTimeSpanOrCountWithMinimum(CronTime time, TimeSpan timeSpan, int minCount, int maxCount)
        {
            return new GenerationRequest(time, timeSpan, minCount, maxCount);
        }

        public CronTime CronTime { get; }
        public TimeSpan? TimeSpan { get; }
        public int? MinCount { get; }
        public int? MaxCount { get; }

        private GenerationRequest(CronTime cronTime, TimeSpan? timeSpan, int? minCount, int? maxCount)
        {
            CronTime = cronTime;
            TimeSpan = timeSpan;
            MinCount = minCount;
            MaxCount = maxCount;
        }
    }
}