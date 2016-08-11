using System;
using System.Reactive.Concurrency;

namespace Rogero.SchedulingLibrary.Scheduling
{
    public class DateTimeRepositoryRx : IDateTimeRepository
    {
        public IScheduler Scheduler { get; set; }

        public DateTimeRepositoryRx(IScheduler scheduler)
        {
            Scheduler = scheduler;
        }

        public DateTime Now()
        {
            return Scheduler.Now.DateTime;
        }

        public DateTime UtcNow()
        {
            return Scheduler.Now.UtcDateTime;
        }
    }
}