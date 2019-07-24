using System.Collections.Generic;
using System.Reflection.Emit;

namespace System.Reflection
{
    internal class CachedActivator
    {
        private static readonly BindingFlags privateConstructor = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        private static readonly Dictionary<Type, CreateInstanceDelegate> delegateCache = new Dictionary<Type, CreateInstanceDelegate>();

        internal static TInstance CreateInstance<TInstance>()
            => (TInstance)CreateInstance(typeof(TInstance));

        internal static object CreateInstance(Type type)
            => GetCreateInstanceDelegate(type).Invoke();

        private static CreateInstanceDelegate GetCreateInstanceDelegate(Type type)
        {
            if (!delegateCache.TryGetValue(type, out CreateInstanceDelegate @delegate))
            {
                lock (type)
                {
                    if (!delegateCache.TryGetValue(type, out @delegate))
                    {
                        var createMethod = new DynamicMethod($"Create_{type.Name}", type, Type.EmptyTypes);
                        var defaultConstructor = type.GetConstructor(privateConstructor, null, Type.EmptyTypes, null);

                        var ilGenerator = createMethod.GetILGenerator();
                        ilGenerator.Emit(OpCodes.Newobj, defaultConstructor);
                        ilGenerator.Emit(OpCodes.Ret);

                        @delegate = (CreateInstanceDelegate)createMethod.CreateDelegate(typeof(CreateInstanceDelegate));
                        delegateCache[type] = @delegate;
                    }
                }
            }

            return @delegate;
        }

        private delegate object CreateInstanceDelegate();
    }
}
