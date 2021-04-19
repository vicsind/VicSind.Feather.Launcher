using System;
using System.Collections.Generic;

namespace Updater.Extensions
{
    public static class ListExtensions
    {
        /// <summary>
        /// Возвращает рандомно отсортированный список элементов IList
        /// </summary>
        public static IList<T> Shuffle<T>(this IEnumerable<T> list)
        {
            IList<T> result = new List<T>(list);
            int n = result.Count;
            while (n > 1)
            {
                n--;
                int k = Random(n + 1);
                T value = result[k];
                result[k] = result[n];
                result[n] = value;
            }

            return result;
        }


        private static int Random(int min, int max)
        {
            lock (Rand)
                return Rand.Next(min, max);
        }

        private static int Random(int max)
        {
            return Random(0, max);
        }

        private static readonly Random Rand = new Random();
    }
}
