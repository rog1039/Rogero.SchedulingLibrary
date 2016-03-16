using System.Collections.Generic;

namespace Rogero.SchedulingLibrary
{
    public class CronTimeGenerator
    {
        public static IEnumerable<CronTime> Generate(CronTime cronTime)
        {
            while (true)
            {
                cronTime = cronTime.GetNext();
                yield return cronTime;
            }
        }
    }
}