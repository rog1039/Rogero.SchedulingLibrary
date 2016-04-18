using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Rogero.SchedulingLibrary.Infrastructure;

namespace Rogero.SchedulingLibrary.Streams
{
    public abstract class CronTimeStreamBase : IEnumerable<CronTime>
    {
        public abstract CronTimeStreamBase AdvanceTo(DateTime dateTime);

        public abstract IEnumerator<CronTime> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public static CronTimeStreamBase Combine(DateTime dateTime, params CronTimeStreamBase[] streams)
        {
            var streamCombination = new CronTimeStreamCombination(dateTime, streams);
            return streamCombination;
        }
    }

    public class CronTimeStreamCombination : CronTimeStreamBase
    {
        private readonly DateTime _dateTime;
        private readonly List<CronTimeStreamBase> _cronTimeStreams;

        public CronTimeStreamCombination(DateTime dateTime, IEnumerable<CronTimeStreamBase> cronTimeStreams)
        {
            _dateTime = dateTime;
            _cronTimeStreams = new List<CronTimeStreamBase>(cronTimeStreams);
        }

        public CronTimeStreamCombination(DateTime dateTime, params CronTimeStreamBase[] cronTimeStreams)
        {
            _dateTime = dateTime;
            _cronTimeStreams = new List<CronTimeStreamBase>(cronTimeStreams);
        }

        public override CronTimeStreamBase AdvanceTo(DateTime dateTime)
        {
            return new CronTimeStreamCombination(dateTime, _cronTimeStreams);
        }

        public override IEnumerator<CronTime> GetEnumerator()
        {
            return CronTimeStreamCombinationGenerator.Generate(_dateTime, _cronTimeStreams).GetEnumerator();
        }
    }

    public class CronTimeStreamCombinationGenerator
    {
        public static IEnumerable<CronTime> Generate(DateTime dateTime, IList<CronTimeStreamBase> cronTimeStreams)
        {
            var priorityList = CreateCronTimePriorityList(dateTime, cronTimeStreams);

            while (true)
            {
                var dueStreams = priorityList.First();
                var nextStreams =
                    (from stream in dueStreams.Value
                        let nextCronTime = stream.First()
                        let nextStream = stream.AdvanceTo(nextCronTime.DateTime.Value)
                        select new {CronTime = nextCronTime, Stream = nextStream})
                        .ToList();
                
                priorityList.AddRange(nextStreams, s => s.CronTime, s => s.Stream);
                priorityList.Remove(dueStreams.Key);
                yield return dueStreams.Key;
            }
        }
        
        private static SortedDictionary<CronTime, IList<CronTimeStreamBase>> CreateCronTimePriorityList(
            DateTime dateTime, IList<CronTimeStreamBase> cronTimeStreams)
        {
            var cronTimes = cronTimeStreams
                .ToSortedDictionaryMany(z => z.AdvanceTo(dateTime).First(),
                                        z => z.AdvanceTo(z.AdvanceTo(dateTime).First().DateTime.Value));
            return cronTimes;
        }
    }
}