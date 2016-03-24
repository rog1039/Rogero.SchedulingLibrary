namespace Rogero.SchedulingLibrary
{
    public partial class CronTime
    {
        public bool Equals(CronTime other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(CronTemplate, other.CronTemplate) && Equals(Time, other.Time);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CronTime)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((CronTemplate != null ? CronTemplate.GetHashCode() : 0) * 397) ^ (Time != null ? Time.GetHashCode() : 0);
            }
        }

        public static bool operator ==(CronTime left, CronTime right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CronTime left, CronTime right)
        {
            return !Equals(left, right);
        }

        public static bool operator >(CronTime c1, CronTime c2)
        {
            return c1.Time > c2.Time;
        }

        public static bool operator <(CronTime c1, CronTime c2)
        {
            return c1.Time < c2.Time;
        }

        public int CompareTo(CronTime other)
        {
            return this.Time.CompareTo(other.Time);
        }

        public int CompareTo(object obj)
        {
            if (obj.GetType() != this.GetType()) return -1;
            return CompareTo((CronTime)obj);
        }
    }
}
