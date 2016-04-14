using System;
using System.Collections.Generic;
using Rogero.SchedulingLibrary.Infrastructure;

namespace Rogero.SchedulingLibrary
{
    public class CronTemplate : IEquatable<CronTemplate>
    {
        public EquatableList Minutes { get; } = new EquatableList();
        public EquatableList Hours { get; } = new EquatableList();
        public EquatableList DaysOfMonth { get; } = new EquatableList();
        public EquatableList Months { get; } = new EquatableList();
        public EquatableList DaysOfWeek { get; } = new EquatableList();

        public CronTemplate(IList<int> minutes, IList<int> hours, IList<int> daysOfMonth, IList<int> months, IList<int> dayOfWeek)
        {
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
            return Minutes.Equals(other.Minutes) && Hours.Equals(other.Hours) && DaysOfMonth.Equals(other.DaysOfMonth) && Months.Equals(other.Months) && DaysOfWeek.Equals(other.DaysOfWeek);
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
                var hashCode = Minutes.GetHashCode();
                hashCode = (hashCode*397) ^ Hours.GetHashCode();
                hashCode = (hashCode*397) ^ DaysOfMonth.GetHashCode();
                hashCode = (hashCode*397) ^ Months.GetHashCode();
                hashCode = (hashCode*397) ^ DaysOfWeek.GetHashCode();
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
