using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;

namespace RemoteMusicPlayerClient.Utility
{
    public static class Enumerable
    {
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            return source.Shuffle(new Random());
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random random)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (random == null)
            {
                throw new ArgumentNullException(nameof(random));
            }

            return source.ShuffleIterator(random);
        }

        private static IEnumerable<T> ShuffleIterator<T>(
            this IEnumerable<T> source, Random random)
        {
            var buffer = source is List<T> list ? list : source.ToList();
            for (var i = 0; i < buffer.Count; i++)
            {
                var j = random.Next(i, buffer.Count);
                yield return buffer[j];

                buffer[j] = buffer[i];
            }
        }
    }
}