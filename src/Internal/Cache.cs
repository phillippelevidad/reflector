using System;
using System.Collections.Concurrent;

namespace Internal
{
    internal static class Cache
    {
        private static readonly object mappingLock = new object();
        private static readonly ConcurrentDictionary<string, Mapping> mappingCache = new ConcurrentDictionary<string, Mapping>();

        private static readonly object constructorAndSettersLock = new object();
        private static readonly ConcurrentDictionary<string, TypeConstructorAndSetters> constructorAndSettersCache = new ConcurrentDictionary<string, TypeConstructorAndSetters>();

        internal static Mapping GetMapping(Type sourceType, Type targetType)
        {
            var key = sourceType.AssemblyQualifiedName + targetType.AssemblyQualifiedName;

            if (!mappingCache.TryGetValue(key, out Mapping mapping))
            {
                lock (mappingLock)
                {
                    if (!mappingCache.TryGetValue(key, out mapping))
                    {
                        mapping = Mapping.BuildFor(sourceType, targetType);
                        mappingCache[key] = mapping;
                    }
                }
            }

            return mapping;
        }

        internal static TypeConstructorAndSetters GetTypeConstructorAndSetters(Type type)
        {
            var key = type.AssemblyQualifiedName;

            if (!constructorAndSettersCache.TryGetValue(key, out TypeConstructorAndSetters constructorAndSetters))
            {
                lock (constructorAndSettersLock)
                {
                    if (!constructorAndSettersCache.TryGetValue(key, out constructorAndSetters))
                    {
                        constructorAndSetters = TypeConstructorAndSetters.BuildFor(type);
                        constructorAndSettersCache[key] = constructorAndSetters;
                    }
                }
            }

            return constructorAndSetters;
        }
    }
}
