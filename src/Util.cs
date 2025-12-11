using System;
using System.Collections.Generic;
using UnityEngine;

namespace Allergies
{
    internal static class Util
    {
        public static void Deconstruct<T1, T2>(this KeyValuePair<T1, T2> tuple, out T1 key, out T2 value)
        {
            key = tuple.Key;
            value = tuple.Value;
        }

        public static T MinBy<T>(this IEnumerable<T> enumerable, Func<T, float> predicate)
        {
            T minItem = default!;
            float minValue = float.PositiveInfinity;
            foreach (var item in enumerable)
            {
                float value = predicate(item);
                if (value < minValue)
                {
                    minValue = value;
                    minItem = item;
                }
            }
            return minItem;
        }

        public static Vector2 Average<T>(this IEnumerable<T> enumerable, Func<T, Vector2> predicate)
        {
            Vector2 sum = default!;
            int count = 0;
            foreach (var item in enumerable)
            {
                sum += predicate.Invoke(item);
                count++;
            }
            return sum / count;
        }
    }
}
