using System.Collections.Generic;
using System.Linq;

namespace Rogero.SchedulingLibrary.Infrastructure
{
    public class EquatableList : List<int>
    {
        public bool Equals(EquatableList other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return this.SequenceEqual(other);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((EquatableList)obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}