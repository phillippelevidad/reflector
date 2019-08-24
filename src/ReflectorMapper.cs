using Internal;

namespace System.Reflection
{
    public static class ReflectorMapper
    {
        public static TTarget Map<TTarget>(object source) where TTarget : class
        {
            var target = Reflector.Create<TTarget>().GetInstance();
            Map(source, target);
            return target as TTarget;
        }

        public static object Map(object source, Type targetType)
        {
            var target = Reflector.Create(targetType).GetInstance();
            Map(source, target);
            return target;
        }

        public static void Map(object source, object target)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (target == null)
                throw new ArgumentNullException(nameof(target));

            var mapping = ExpressionCache.GetMapping(source.GetType(), target.GetType());
            mapping.Map(source, target);
        }
    }
}
