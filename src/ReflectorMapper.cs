using Internal;

namespace System.Reflection
{
    public static class ReflectorMapper
    {
        public static TTarget Map<TSource, TTarget>(TSource source)
            where TSource : class
            where TTarget : class
        {
            var target = Reflector.Create<TSource>().GetInstance();
            Map(source, target);
            return target as TTarget;
        }

        public static void Map<TSource, TTarget>(TSource source, TTarget target)
            where TSource : class
            where TTarget : class
        {
            var mapping = ExpressionCache.GetMapping<TSource, TTarget>();
            mapping.Map(source, target);
        }
    }
}
