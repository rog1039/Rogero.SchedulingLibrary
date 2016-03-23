using System;
using System.Reactive.Concurrency;

namespace Rogero.SchedulingLibrary.Scheduling
{
    public class DateTimeRepositoryRx : IDateTimeRepository
    {
        public IScheduler TestScheduler { get; set; }

        public DateTimeRepositoryRx(IScheduler testScheduler)
        {
            TestScheduler = testScheduler;
        }

        public DateTime Now()
        {
            return TestScheduler.Now.DateTime;
        }
    }
}