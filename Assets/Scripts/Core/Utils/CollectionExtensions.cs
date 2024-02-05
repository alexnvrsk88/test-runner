using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Runner.Core.Utils
{
    public static class CollectionExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveBySwap<T>(this IList<T> list, int index)
        {
            var lastIndex = list.Count - 1;

            if (index != lastIndex)
            {
                list[index] = list[lastIndex];
            }

            list.RemoveAt(lastIndex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveBySwap<T>(this IList<T> list, T item)
        {
            var index = list.IndexOf(item);
            if (index < 0) return;
            list.RemoveBySwap(index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Find<T>(this IReadOnlyList<T> readOnlyList, Predicate<T> predicate)
        {
            switch (readOnlyList)
            {
                case List<T> list:
                {
                    return list.Find(predicate);
                }
                default:
                {
                    var func = new Func<T, bool>(predicate);
                    return readOnlyList.FirstOrDefault(func);
                }
            }
        }
    }
}