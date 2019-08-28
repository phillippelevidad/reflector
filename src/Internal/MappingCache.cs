using System;
using System.Collections.Concurrent;

namespace Internal
{
    internal static class MappingCache
    {
        private static readonly object cacheLock = new object();
        private static readonly ConcurrentDictionary<string, Mapping> cache = new ConcurrentDictionary<string, Mapping>();

        internal static Mapping GetMapping(Type sourceType, Type targetType)
        {
            var key = GetKey(sourceType, targetType);

            if (!cache.TryGetValue(key, out Mapping mapping))
            {
                lock (cacheLock)
                {
                    if (!cache.TryGetValue(key, out mapping))
                    {
                        mapping = Mapping.BuildFor(sourceType, targetType);
                        cache[key] = mapping;
                    }
                }
            }

            return mapping;
        }

        private static string GetKey(Type source, Type target) => source.AssemblyQualifiedName + target.AssemblyQualifiedName;
    }
}
