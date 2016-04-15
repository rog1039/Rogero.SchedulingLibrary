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
                var streams = priorityList.First();
                var nextStreams2 = (from stream in streams.Value
                    let nextCronTime = stream.First()
                    let nextStream = stream.AdvanceTo(nextCronTime.DateTime.Value)
                    select new {CronTime = nextCronTime, Stream = nextStream}).ToList();
                var nextStreams = streams
                    .Value
                    .Select(z => new {CronTime = z.First(), Stream = z.AdvanceTo(z.First().DateTime.Value)})
                    .ToList();
                priorityList.AddRange(nextStreams2, s => s.CronTime, s => s.Stream);
                priorityList.Remove(streams.Key);
                yield return streams.Key;
            }
        }


        private static SortedDictionary<CronTime, IList<CronTimeStreamBase>> CreateCronTimePriorityList(
            DateTime dateTime, IList<CronTimeStreamBase> cronTimeStreams)
        {
            var cronTImes2 = from stream in cronTimeStreams
                let advancedStream = stream.AdvanceTo(dateTime)
                let nextTime = advancedStream.First()
                let furtherAdvancedStream = stream.AdvanceTo(nextTime.DateTime.Value)
                select new {Key = nextTime, Stream = furtherAdvancedStream};
            var dict = new SortedDictionary<CronTime, IList<CronTimeStreamBase>>();
            foreach (var kvp in cronTImes2)
            {
                dict.AddToDictionary(kvp.Key, kvp.Stream);
            }
            return dict;

            var cronTimes = cronTimeStreams
                .ToSortedDictionaryMany(z => z.AdvanceTo(dateTime).First(),
                                        z => z.AdvanceTo(z.AdvanceTo(dateTime).First().DateTime.Value));
            return cronTimes;
        }
    }
}