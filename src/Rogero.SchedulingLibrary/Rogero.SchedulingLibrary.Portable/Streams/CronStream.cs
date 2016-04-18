using System;
using System.Collections.Generic;
using System.Linq;
using Rogero.SchedulingLibrary.Infrastructure;

namespace Rogero.SchedulingLibrary.Streams
{
    public static class CronStream
    {
        public static CronTimeStreamBase CreateSchedule(DaysOfWeek daysOfWeek, string input, DateTime dateTime = default(DateTime))
        {
            return CronScheduleParser.CreateCronTimeStream(daysOfWeek, input, dateTime);
        }

        public static CronTimeStreamBase Combine(DateTime dateTime, IEnumerable<CronTimeStreamBase> streams)
        {
            var stream = new CronTimeStreamCombination(dateTime, streams);
            return stream;
        }

        public static CronTimeStreamBase Combine(DateTime dateTime, params CronTimeStreamBase[] streams)
        {
            var stream = new CronTimeStreamCombination(dateTime, streams);
            return stream;
        }
    }
}