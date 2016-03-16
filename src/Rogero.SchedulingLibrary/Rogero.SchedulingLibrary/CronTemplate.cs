using System.Collections.Generic;
using System.Text;

namespace Rogero.SchedulingLibrary
{
    public class CronTemplate
    {
        public IList<int> Minutes { get; }
        public IList<int> Hours { get; }
        public IList<int> DaysOfMonth { get; }
        public IList<int> Months { get; }
        public IList<int> DayOfWeek { get; }

        public CronTemplate(IList<int> minutes, IList<int> hours, IList<int> daysOfMonth, IList<int> months, IList<int> dayOfWeek)
        {
            Minutes = minutes;
            Hours = hours;
            DaysOfMonth = daysOfMonth;
            Months = months;
            DayOfWeek = dayOfWeek;
        }
    }
}
