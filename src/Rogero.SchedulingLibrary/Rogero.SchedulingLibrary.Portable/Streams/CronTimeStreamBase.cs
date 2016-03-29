using System;
using System.Collections;
using System.Collections.Generic;

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
    }
}
