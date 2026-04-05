using System;
using System.Collections.Generic;

namespace D365FL.Dataverse.PluginHelper.Core.IEnumerableExtensions
{
    public static class ChunkifyExtension
    {
        public static IEnumerable<List<T>> Chunkify<T>(this IEnumerable<T> source, int chunkSize)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (chunkSize <= 0) throw new ArgumentOutOfRangeException(nameof(chunkSize), "Chunk size must be greater than zero.");

            var chunk = new List<T>(chunkSize);
            foreach (var item in source)
            {
                chunk.Add(item);
                if (chunk.Count == chunkSize)
                {
                    yield return chunk;
                    chunk = new List<T>(chunkSize);
                }
            }

            if (chunk.Count > 0)
                yield return chunk;
        }
    }
}
