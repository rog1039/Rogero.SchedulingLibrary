using System;
using System.Collections.Generic;
using System.Text;

namespace Rogero.SchedulingLibrary
{
    public static class ListExtensionMethods2
    {
        public static void AddRange<T>(this IList<T> list, IEnumerable<T> newItems)
        {
            foreach (var newItem in newItems)
            {
                list.Add(newItem);
            }
        }
    }
}
