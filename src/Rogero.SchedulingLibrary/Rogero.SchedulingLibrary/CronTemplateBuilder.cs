using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Rogero.SchedulingLibrary
{
    public class CronTemplateBuilder
    {
        private IList<int> Minutes;
        private IList<int> Hours;
        private IList<int> DaysOfMonth;
        private IList<int> Months;
        private IList<int> DaysOfWeek;

        public CronTemplateBuilder WithAllMinutes()
        {
            Minutes = ZeroTo59;
            return this;
        }

        public CronTemplateBuilder WithMinutes(params int[] minutes)
        {
            ArrayMustHaveLengthGreaterThanZero(minutes);
            Minutes = new List<int>(minutes);
            return this;
        }

        public CronTemplateBuilder WithAllHours()
        {
            Hours = ZeroTo23;
            return this;
        }

        public CronTemplateBuilder WithHours(params int[] hours)
        {
            ArrayMustHaveLengthGreaterThanZero(hours);
            Hours = new List<int>(hours);
            return this;
        }

        public CronTemplateBuilder WithAllDaysOfMonth()
        {
            DaysOfMonth = OneTo31;
            return this;
        }

        public CronTemplateBuilder WithDaysOfMonth(params int[] daysOfMonth)
        {
            ArrayMustHaveLengthGreaterThanZero(daysOfMonth);
            DaysOfMonth = new List<int>(daysOfMonth);
            return this;
        }

        public CronTemplateBuilder WithAllMonths()
        {
            Months = OneTo12;
            return this;
        }

        public CronTemplateBuilder WithMonths(params int[] months)
        {
            ArrayMustHaveLengthGreaterThanZero(months);
            DaysOfWeek = new List<int>(months);
            return this;
        }

        public CronTemplateBuilder WithAllDaysOfWeek()
        {
            DaysOfWeek = ZeroTo6;
            return this;
        }

        public CronTemplateBuilder WithWeekDays()
        {
            DaysOfWeek = OneTo5;
            return this;
        }

        public CronTemplateBuilder WithDaysOfWeek(params int[] daysOfWeek)
        {
            ArrayMustHaveLengthGreaterThanZero(daysOfWeek);
            DaysOfWeek = new List<int>(daysOfWeek);
            return this;
        }

        private void ArrayMustHaveLengthGreaterThanZero(int[] ints)
        {
            if(ints == null || ints.Length == 0)
                throw new InvalidDataException("Array cannot be null or have no elements. When providing values to the CronTemplateBuilder, you must specify at least one integer.");
        }

        public CronTemplate BuildCronTemplate()
        {
            if(Minutes == null || Hours == null || DaysOfMonth == null || Months == null || DaysOfWeek == null) 
                throw new ArgumentNullException("Values must be provided for all sections of the CronTemplate: Minutes, Hours, DaysOfMonth, Months, and DaysOfWeek");

            return new CronTemplate(Minutes, Hours, DaysOfMonth, Months, DaysOfWeek);
        }

        private static readonly IList<int> ZeroTo59 = Enumerable.Range(0, 60).ToList();
        private static readonly IList<int> ZeroTo23 = Enumerable.Range(0, 24).ToList();
        private static readonly IList<int> OneTo31 = Enumerable.Range(1, 31).ToList();
        private static readonly IList<int> OneTo12 = Enumerable.Range(1, 12).ToList();
        private static readonly IList<int> ZeroTo6 = Enumerable.Range(0, 7).ToList();
        private static readonly IList<int> OneTo5 = Enumerable.Range(1, 5).ToList();
    }
}