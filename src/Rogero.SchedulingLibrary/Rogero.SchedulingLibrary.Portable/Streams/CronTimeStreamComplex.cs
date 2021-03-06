using System;
using System.Collections.Generic;
using Rogero.SchedulingLibrary.Generators;

namespace Rogero.SchedulingLibrary.Streams
{
    public class CronTimeStreamComplex : CronTimeStreamBase
    {
        private readonly DateTime _dateTime;
        public CronTemplate[] CronTemplates { get; }

        public CronTimeStreamComplex(DateTime dateTime, params CronTemplate[] cronTemplates)
        {
            _dateTime = dateTime;
            CronTemplates = cronTemplates;
        }

        public override CronTimeStreamBase AdvanceTo(DateTime dateTime)
        {
            return new CronTimeStreamComplex(dateTime, CronTemplates);
        }

        public override IEnumerator<CronTime> GetEnumerator()
        {
            return CronTimeGenerator.Generate(_dateTime, CronTemplates).GetEnumerator();
        }
    }
}