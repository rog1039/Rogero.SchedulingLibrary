using System;
using System.Collections.Generic;
using Rogero.SchedulingLibrary.Generators;

namespace Rogero.SchedulingLibrary.Streams
{
    public class CronTimeStreamSimple : CronTimeStreamBase
    {
        private readonly CronTime _cronTime;

        public CronTimeStreamSimple(CronTemplate template, DateTime datetime)
        {
            _cronTime = new CronTime(template, datetime);
        }

        public override CronTimeStreamBase AdvanceTo(DateTime dateTime)
        {
            return new CronTimeStreamSimple(_cronTime.CronTemplate, dateTime);
        }

        public override IEnumerator<CronTime> GetEnumerator()
        {
            return _cronTime.ToSeries().GetEnumerator();
        }
    }
}