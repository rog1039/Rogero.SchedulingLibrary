namespace Rogero.SchedulingLibrary
{
    public partial class Time
    {
        public bool Equals(Time other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Year == other.Year && Month == other.Month && Day == other.Day && Hour == other.Hour && Minute == other.Minute && Second == other.Second;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Time) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Year;
                hashCode = (hashCode*397) ^ Month;
                hashCode = (hashCode*397) ^ Day;
                hashCode = (hashCode*397) ^ Hour;
                hashCode = (hashCode*397) ^ Minute;
                hashCode = (hashCode*397) ^ Second;
                return hashCode;
            }
        }

        public static bool operator ==(Time left, Time right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Time left, Time right)
        {
            return !Equals(left, right);
        }
        
        public static bool operator >(Time c1, Time c2)
        {
            return c1.ToDateTime() > c2.ToDateTime();
        }

        public static bool operator <(Time c1, Time c2)
        {
            return c1.ToDateTime() < c2.ToDateTime();
        }

        public static bool operator >=(Time c1, Time c2)
        {
            return c1.ToDateTime() >= c2.ToDateTime();
        }

        public static bool operator <=(Time c1, Time c2)
        {
            return c1.ToDateTime() <= c2.ToDateTime();
        }

        public int CompareTo(Time time)
        {
            int result;

            result = Year.CompareTo(time.Year);
            if (result != 0) return result;

            result = Month.CompareTo(time.Month);
            if (result != 0) return result;

            result = Day.CompareTo(time.Day);
            if (result != 0) return result;

            result = Hour.CompareTo(time.Hour);
            if (result != 0) return result;

            result = Minute.CompareTo(time.Minute);
            if (result != 0) return result;

            result = Second.CompareTo(time.Second);
            return result;
        }
    }
}
