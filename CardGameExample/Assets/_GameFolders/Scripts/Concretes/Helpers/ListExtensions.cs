using System;
using System.Collections.Generic;
using System.Threading;

namespace CardGame.Helpers
{
    public static class ListExtensions
    {
        static readonly ThreadLocal<Random> random = new ThreadLocal<Random>(() => new Random());

        public static void Shuffle<T>(this List<T> list, int? seed = null)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));

            Random rng = seed.HasValue ? new Random(seed.Value) : random.Value;

            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }
    }
}