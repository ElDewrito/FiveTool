using System;
using System.Collections.Generic;

namespace FiveLib.Common
{
    internal static class BinarySearch
    {
        public static int Search<T>(IList<T> values, T value)
            where T: IComparable<T>
        {
            return Search(values, value, v => v);
        }

        public static int Search<T>(IList<T> values, T value, Comparison<T> comparer)
        {
            return Search(values, value, comparer, v => v);
        }

        public static int Search<TSource, TKey>(IList<TSource> values, TKey value, Func<TSource, TKey> selector)
            where TKey : IComparable<TKey>
        {
            return Search(values, value, (x, y) => x.CompareTo(y), selector);
        }

        public static int Search<TSource, TKey>(IList<TSource> values, TKey value, Comparison<TKey> comparer, Func<TSource, TKey> selector)
        {
            var lower = 0;
            var upper = values.Count - 1;
            while (lower <= upper)
            {
                var middle = (lower + upper) / 2;
                var testValue = selector(values[middle]);
                var comparison = comparer(value, testValue);
                if (comparison == 0)
                    return middle;
                if (comparison < 0)
                    upper = middle - 1;
                else
                    lower = middle + 1;
            }
            return ~lower;
        }
    }
}