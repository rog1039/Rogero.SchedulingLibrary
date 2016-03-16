using System;

namespace Rogero.SchedulingLibrary
{
    public class CronTime
    {
        public CronTemplate CronTemplate { get; }
        public Time Time { get; }

        public CronTime(CronTemplate cronTemplate, Time time)
        {
            CronTemplate = cronTemplate;
            Time = time;
        }

        public CronTime(CronTemplate cronTemplate, DateTime datetime)
        {
            CronTemplate = cronTemplate;
            Time = new Time(datetime);
        }

        public CronTime GetNext()
        {
            return CronTimeIncrementor.Increment(this);
        }
    }
}