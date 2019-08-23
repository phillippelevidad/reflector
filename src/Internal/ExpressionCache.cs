using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Internal
{
    internal static class ExpressionCache
    {
        private static readonly ConcurrentDictionary<string, Constructor> constructorCache = new ConcurrentDictionary<string, Constructor>();
        private static readonly ConcurrentDictionary<string, Setter> setterCache = new ConcurrentDictionary<string, Setter>();
        private static readonly ConcurrentDictionary<string, Mapping> mappingCache = new ConcurrentDictionary<string, Mapping>();

        internal static Constructor GetConstructor<TInstance>()
        {
            var type = typeof(TInstance);
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

        internal static Setter GetSetter<TInstance>(string memberName)
        {
            var type = typeof(TInstance);
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

        internal static Mapping GetMapping<TSource, TTarget>()
        {
            var key = GetKey(typeof(TSource), typeof(TTarget));

            if (!mappingCache.TryGetValue(key, out Mapping mapping))
            {
                var mappingLock = new MappingLock<TSource, TTarget>();

                lock (mappingLock)
                {
                    if (!mappingCache.TryGetValue(key, out mapping))
                    {
                        mapping = Mapping.BuildFor<TSource, TTarget>();
                        mappingCache[key] = mapping;
                    }
                }
            }

            return mapping;
        }

        private static string GetKey(Type type) => type.AssemblyQualifiedName;
        private static string GetKey(Type type, string memberName) => $"{type.AssemblyQualifiedName}_{memberName}";
        private static string GetKey(Type source, Type target) => $"{source.AssemblyQualifiedName}|{target.AssemblyQualifiedName}";

        private class MappingLock<TSource, TTarget> { }
    }
}
