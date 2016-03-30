using System.Collections.Generic;

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