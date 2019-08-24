using System;
using System.Collections.Concurrent;

namespace Internal
{
    internal static class ExpressionCache
    {
        private static readonly object constructorLock = new object();
        private static readonly object setterLock = new object();
        private static readonly object mappingLock = new object();

        private static readonly ConcurrentDictionary<string, Constructor> constructorCache = new ConcurrentDictionary<string, Constructor>();
        private static readonly ConcurrentDictionary<string, Setter> setterCache = new ConcurrentDictionary<string, Setter>();
        private static readonly ConcurrentDictionary<string, Mapping> mappingCache = new ConcurrentDictionary<string, Mapping>();

        internal static Constructor GetConstructor(Type type)
        {
            var key = GetKey(type);

            if (!constructorCache.TryGetValue(key, out Constructor constructor))
            {
                lock (constructorLock)
                {
                    if (!constructorCache.TryGetValue(key, out constructor))
                    {
                        var result = Constructor.BuildFor(type)
                            .OnFailure(error => throw new InvalidOperationException(error));

                        constructor = result.Value;
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
                lock (setterLock)
                {
                    if (!setterCache.TryGetValue(key, out setter))
                    {
                        var result = Setter.BuildFor(type, memberName)
                            .OnFailure(error => throw new InvalidOperationException(error));

                        setter = result.Value;
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
        private static string GetKey(Type type, string memberName) => type.AssemblyQualifiedName + memberName;
        private static string GetKey(Type source, Type target) => source.AssemblyQualifiedName + target.AssemblyQualifiedName;
    }
}
