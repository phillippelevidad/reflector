using System;
using System.Collections.Generic;

namespace src
{
    public class ReflectorMapper
    {
        public static TTarget Map<TSource, TTarget>(TSource source)
        {
            throw new NotImplementedException();
        }

        public static void Map<TSource, TTarget>(TSource source, TTarget target)
        {
            throw new NotImplementedException();
        }
    }

    public class ReflectorMapper<TSource, TTarget>
        where TSource : class
        where TTarget : class
    {
        private readonly Type sourceType;
        private readonly Type targetType;

        private ReflectorMapper()
        {
            sourceType = typeof(TSource);
            targetType = typeof(TTarget);
            Key = $"{sourceType.AssemblyQualifiedName}_{targetType.AssemblyQualifiedName}";
        }

        public static ReflectorMapper<TSource, TTarget> Create()
        {
            return new ReflectorMapper<TSource, TTarget>();
        }

        public string Key { get; }

        private static Dictionary<object, object> BuildMapping()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return $"{sourceType.FullName}_{targetType.FullName}";
        }
    }
}
