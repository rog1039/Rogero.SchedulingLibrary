using System;
using System.Collections.Generic;
using Rogero.SchedulingLibrary.Generators;

namespace Rogero.SchedulingLibrary.Streams
{
    public class CronTimeStreamComplex : CronTimeStreamBase
    {
        private readonly DateTime _dateTime;
        private readonly CronTemplate[] _cronTemplates;

        public CronTimeStreamComplex(DateTime dateTime, params CronTemplate[] cronTemplates)
        {
            _dateTime = dateTime;
            _cronTemplates = cronTemplates;
        }

        public override CronTimeStreamBase AdvanceTo(DateTime dateTime)
        {
            return new CronTimeStreamComplex(dateTime, _cronTemplates);
        }

        public override IEnumerator<CronTime> GetEnumerator()
        {
            return CronTimeGenerator.Generate(_dateTime, _cronTemplates).GetEnumerator();
        }
    }
}