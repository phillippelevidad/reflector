using System;
using System.Collections.Concurrent;

namespace Internal
{
    internal static class TypeConstructorAndSettersCache
    {
        private static readonly object cacheLock = new object();
        private static readonly ConcurrentDictionary<string, TypeConstructorAndSetters> cache = new ConcurrentDictionary<string, TypeConstructorAndSetters>();

        internal static TypeConstructorAndSetters GetTypeConstructorAndSetters(Type type)
        {
            var key = type.AssemblyQualifiedName;

            if (!cache.TryGetValue(key, out TypeConstructorAndSetters constructorAndSetters))
            {
                lock (cacheLock)
                {
                    if (!cache.TryGetValue(key, out constructorAndSetters))
                    {
                        constructorAndSetters = TypeConstructorAndSetters.BuildFor(type);
                        cache[key] = constructorAndSetters;
                    }
                }
            }

            return constructorAndSetters;
        }
    }
}
