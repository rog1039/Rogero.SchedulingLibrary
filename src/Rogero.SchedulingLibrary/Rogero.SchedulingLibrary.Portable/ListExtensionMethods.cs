using System;
using System.Collections.Generic;
using System.Text;

namespace Rogero.SchedulingLibrary
{
    public static class ListExtensionMethods
    {
        public static int ClosestLowerIndexOf(this IList<int> possibleValues, int value)
        {
            bool found = false;
            for (int i = 0; i < possibleValues.Count; i++)
            {
                if (value < possibleValues[i]) return i - 1;
            }
            return possibleValues.Count - 1;
        }

        public static void AddRange<T>(this IList<T> list, IEnumerable<T> newItems)
        {
            foreach (var newItem in newItems)
            {
                list.Add(newItem);
            }
        }
    }
}
