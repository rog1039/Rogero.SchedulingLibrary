using System;
using System.Collections.Generic;
using Rogero.SchedulingLibrary.Infrastructure;

namespace Rogero.SchedulingLibrary
{
    public class CronTemplate : IEquatable<CronTemplate>
    {
        public EquatableList Seconds { get; } = new EquatableList();
        public EquatableList Minutes { get; } = new EquatableList();
        public EquatableList Hours { get; } = new EquatableList();
        public EquatableList DaysOfMonth { get; } = new EquatableList();
        public EquatableList Months { get; } = new EquatableList();
        public EquatableList DaysOfWeek { get; } = new EquatableList();

        public CronTemplate(IList<int> seconds, IList<int> minutes, IList<int> hours, IList<int> daysOfMonth, IList<int> months, IList<int> dayOfWeek)
        {
            Seconds.AddRange(seconds);
            Minutes.AddRange(minutes);
            Hours.AddRange(hours);
            DaysOfMonth.AddRange(daysOfMonth);
            Months.AddRange(months);
            DaysOfWeek.AddRange(dayOfWeek);
        }

        public bool Equals(CronTemplate other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Seconds, other.Seconds) && Equals(Minutes, other.Minutes) && Equals(Hours, other.Hours) && Equals(DaysOfMonth, other.DaysOfMonth) && Equals(Months, other.Months) && Equals(DaysOfWeek, other.DaysOfWeek);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CronTemplate) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Seconds != null ? Seconds.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Minutes != null ? Minutes.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Hours != null ? Hours.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (DaysOfMonth != null ? DaysOfMonth.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Months != null ? Months.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (DaysOfWeek != null ? DaysOfWeek.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(CronTemplate left, CronTemplate right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CronTemplate left, CronTemplate right)
        {
            return !Equals(left, right);
        }
    }

}
