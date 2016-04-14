using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Rogero.SchedulingLibrary
{
    public partial class CronTemplateBuilder
    {
        private IList<int> _minutes;
        private IList<int> _hours;
        private IList<int> _daysOfMonth;
        private IList<int> _months;
        private IList<int> _daysOfWeek;

        public CronTemplateBuilder WithAllMinutes()
        {
            _minutes = ZeroTo59;
            return this;
        }

        public CronTemplateBuilder WithMinutes(params int[] minutes)
        {
            ArrayMustHaveLengthGreaterThanZero(minutes);
            _minutes = new List<int>(minutes);
            return this;
        }

        public CronTemplateBuilder WithAllHours()
        {
            _hours = ZeroTo23;
            return this;
        }

        public CronTemplateBuilder WithHours(params int[] hours)
        {
            ArrayMustHaveLengthGreaterThanZero(hours);
            _hours = new List<int>(hours);
            return this;
        }

        public CronTemplateBuilder WithAllDaysOfMonth()
        {
            _daysOfMonth = OneTo31;
            return this;
        }

        public CronTemplateBuilder WithDaysOfMonth(params int[] daysOfMonth)
        {
            ArrayMustHaveLengthGreaterThanZero(daysOfMonth);
            _daysOfMonth = new List<int>(daysOfMonth);
            return this;
        }

        public CronTemplateBuilder WithAllMonths()
        {
            _months = OneTo12;
            return this;
        }

        public CronTemplateBuilder WithMonths(params int[] months)
        {
            ArrayMustHaveLengthGreaterThanZero(months);
            _months = new List<int>(months);
            return this;
        }

        public CronTemplateBuilder WithAllDaysOfWeek()
        {
            _daysOfWeek = ZeroTo6;
            return this;
        }

        public CronTemplateBuilder WithWeekDays()
        {
            _daysOfWeek = OneTo5;
            return this;
        }

        public CronTemplateBuilder WithDaysOfWeek(params int[] daysOfWeek)
        {
            ArrayMustHaveLengthGreaterThanZero(daysOfWeek);
            _daysOfWeek = new List<int>(daysOfWeek);
            return this;
        }

        private void ArrayMustHaveLengthGreaterThanZero(int[] ints)
        {
            if(ints == null || ints.Length == 0)
                throw new InvalidDataException("Array cannot be null or have no elements. When providing values to the CronTemplateBuilder, you must specify at least one integer.");
        }

        public CronTemplateBuilder EveryXMinutes(int period, int min = 0, int max = 59)
        {
            _minutes = Enumerable.Range(min, max).Where(z => z%period == 0).ToList();
            return this;
        }

        public CronTemplateBuilder EveryXHours(int period, int min = 0, int max = 23)
        {
            _hours = Enumerable.Range(min, max).Where(z => z % period == 0).ToList();
            return this;
        }

        public CronTemplateBuilder EveryXDays(int period, int min = 1, int max = 31)
        {
            _daysOfMonth = Enumerable.Range(min, max).Where(z => z % period == 0).ToList();
            return this;
        }

        public CronTemplateBuilder WithEverything()
        {
            return WithAllMinutes().WithAllHours().WithAllDaysOfMonth().WithAllDaysOfWeek().WithAllMonths();
        }

        public CronTemplate Build()
        {
            if(_minutes == null || _hours == null || _daysOfMonth == null || _months == null || _daysOfWeek == null) 
                throw new ArgumentNullException("Values must be provided for all sections of the CronTemplate: Minutes, Hours, DaysOfMonth, Months, and DaysOfWeek");

            return new CronTemplate(_minutes, _hours, _daysOfMonth, _months, _daysOfWeek);
        }

        private static readonly IList<int> ZeroTo59 = Enumerable.Range(0, 60).ToList();
        private static readonly IList<int> ZeroTo23 = Enumerable.Range(0, 24).ToList();
        private static readonly IList<int> OneTo31 = Enumerable.Range(1, 31).ToList();
        private static readonly IList<int> OneTo12 = Enumerable.Range(1, 12).ToList();
        private static readonly IList<int> ZeroTo6 = Enumerable.Range(0, 7).ToList();
        private static readonly IList<int> OneTo5 = Enumerable.Range(1, 5).ToList();
    }
}