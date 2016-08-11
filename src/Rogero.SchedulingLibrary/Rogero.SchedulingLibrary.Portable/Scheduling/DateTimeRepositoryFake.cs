using System;

namespace Rogero.SchedulingLibrary.Scheduling
{
    public class DateTimeRepositoryFake : IDateTimeRepository
    {
        public DateTime DateTime { get; set; }

        public DateTime Now()
        {
            return DateTime;
        }

        public DateTime UtcNow()
        {
            return DateTime;
        }
    }
}