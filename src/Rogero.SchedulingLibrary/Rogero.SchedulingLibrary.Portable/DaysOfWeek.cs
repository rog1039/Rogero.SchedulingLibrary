using System;

namespace Rogero.SchedulingLibrary
{
    [Flags]
    public enum DaysOfWeek
    {
        Sunday = 1,
        Monday = 2,
        Tuesday = 4,
        Wednesday = 8,
        Thursday = 16,
        Friday = 32,
        Saturday = 64,
        MTWT = Monday | Tuesday | Wednesday | Thursday,
        MTWTF = Monday | Tuesday | Wednesday | Thursday | Friday,
        SMTWT = Sunday | Monday | Tuesday | Wednesday | Thursday,
        TWTFS = Tuesday | Wednesday | Thursday | Friday | Saturday,
        Weekdays = MTWTF,
        Weekends = Sunday | Saturday,
        AllDays = Weekdays | Weekends,
        Everyday = AllDays
    }
}