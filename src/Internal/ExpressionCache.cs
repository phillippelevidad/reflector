using System;
using System.Collections.Concurrent;

namespace Internal
{
    internal static class ExpressionCache
    {
        private static readonly ConcurrentDictionary<string, Constructor> constructorCache = new ConcurrentDictionary<string, Constructor>();
        private static readonly ConcurrentDictionary<string, Setter> setterCache = new ConcurrentDictionary<string, Setter>();
        private static readonly ConcurrentDictionary<string, Mapping> mappingCache = new ConcurrentDictionary<string, Mapping>();

        internal static Constructor GetConstructor(Type type)
        {
            var key = GetKey(type);

            if (!constructorCache.TryGetValue(key, out Constructor constructor))
            {
                lock (key)
                {
                    if (!constructorCache.TryGetValue(key, out constructor))
                    {
                        constructor = Constructor.BuildFor(type);
                        constructorCache[key] = constructor;
                    }
                }
            }

            return constructor;
        }

        internal static Setter GetSetter(Type type, string memberName)
        {
            var key = GetKey(type, memberName);

            if (!setterCache.TryGetValue(key, out Setter setter))
            {
                lock (type)
                {
                    if (!setterCache.TryGetValue(key, out setter))
                    {
                        setter = Setter.BuildFor(type, memberName);
                        setterCache[key] = setter;
                    }
                }
            }

            return setter;
        }

        internal static Mapping GetMapping(Type sourceType, Type targetType)
        {
            var key = GetKey(sourceType, targetType);

            if (!mappingCache.TryGetValue(key, out Mapping mapping))
            {
                var mappingLock = new object();

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

        private static string GetKey(Type type) => type.AssemblyQualifiedName;
        private static string GetKey(Type type, string memberName) => $"{type.AssemblyQualifiedName}_{memberName}";
        private static string GetKey(Type source, Type target) => $"{source.AssemblyQualifiedName}|{target.AssemblyQualifiedName}";
    }
}
