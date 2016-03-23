namespace Rogero.SchedulingLibrary
{
    public class IncrementListResult
    {
        public int Value { get; }
        public bool Overflow { get; }
        public bool NoOverflow => !Overflow;

        private IncrementListResult(int value, bool overflow)
        {
            Value = value;
            Overflow = overflow;
        }

        public static IncrementListResult ValueWithOverflow(int value)
        {
            return new IncrementListResult(value, true);
        }

        public static IncrementListResult ValueWithoutOverflow(int value)
        {
            return new IncrementListResult(value, false);
        }
    }
}