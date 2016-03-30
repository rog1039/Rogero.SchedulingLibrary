using System.Collections.Generic;
// ReSharper disable IteratorNeverReturns

namespace Rogero.SchedulingLibrary.Generators
{
    public static class CronTimeExtensionMethods
    {
        public static IEnumerable<CronTime> ToSeries(this CronTime cronTime)
        {
            while (true)
            {
                cronTime = cronTime.GetNext();
                yield return cronTime;
            }
        }
    }
}