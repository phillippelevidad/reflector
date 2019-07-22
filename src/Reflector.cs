using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;

namespace System.Reflection
{
    // Important: you must install package System.Reflection.Emit.Lightweight
    // https://www.nuget.org/packages/System.Reflection.Emit.Lightweight

    public class Reflector
    {
        public static Reflector<T> Create<T>() where T : class
        {
            return Reflector<T>.CreateInstance<T>();
        }
    }

    public class Reflector<T> where T : class
    {
        private readonly Type type;
        private readonly T instance;

        private Reflector()
        {
            type = typeof(T);
            instance = CachedActivator.CreateInstance<T>();
        }

        public T GetInstance()
        {
            return instance;
        }

        public Reflector<T> Set(string name, object value, bool isField = false)
        {
            return isField ? SetField(name, value) : SetProperty(name, value);
        }

        public Reflector<T> SetField(string name, object value)
        {
            CachedSetter.Field<T>(type, name).Invoke(instance, value);
            return this;
        }

        public Reflector<T> SetProperty(string name, object value)
        {
            CachedSetter.Property<T>(type, name).Invoke(instance, value);
            return this;
        }

        public static Reflector<TModel> CreateInstance<TModel>() where TModel : class
        {
            return new Reflector<TModel>();
        }

        #region Cached reflection operations

        private class CachedActivator
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

        private class CachedSetter
        {
            private static readonly BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase;
            private static readonly ConcurrentDictionary<string, Delegate> expressionCache = new ConcurrentDictionary<string, Delegate>();

            internal static Action<TInstance, object> Property<TInstance>(Type type, string propertyName)
            {
                var key = GetKey(type, propertyName);

                if (!expressionCache.TryGetValue(key, out Delegate expression))
                {
                    lock (type)
                    {
                        if (!expressionCache.TryGetValue(key, out expression))
                        {
                            var property = type.GetProperty(propertyName, flags);

                            var target = Expression.Parameter(type, "target");
                            var withValue = Expression.Parameter(typeof(object), "value");

                            var accessProperty = Expression.Property(target, property);
                            var assign = Expression.Assign(accessProperty, Expression.Convert(withValue, property.PropertyType));

                            expression = Expression.Lambda<Action<TInstance, object>>(assign, target, withValue).Compile();
                            expressionCache[key] = expression;
                        }
                    }
                }

                return (Action<TInstance, object>)expression;
            }

            internal static Action<TInstance, object> Field<TInstance>(Type type, string fieldName)
            {
                var key = GetKey(type, fieldName, isField: true);

                if (!expressionCache.TryGetValue(key, out Delegate expression))
                {
                    lock (type)
                    {
                        if (!expressionCache.TryGetValue(key, out expression))
                        {
                            var field = type.GetField(fieldName, flags);

                            var target = Expression.Parameter(type, "target");
                            var withValue = Expression.Parameter(typeof(object), "value");

                            var accessField = Expression.Field(target, field);
                            var assign = Expression.Assign(accessField, Expression.Convert(withValue, field.FieldType));

                            expression = Expression.Lambda<Action<TInstance, object>>(assign, target, withValue).Compile();
                            expressionCache[key] = expression;
                        }
                    }
                }

                return (Action<TInstance, object>)expression;
            }

            private static string GetKey(Type type, string propertyOrField, bool isField = false)
            {
                var identifier = isField ? "f" : "p";
                return $"{type.FullName}_{identifier}_{propertyOrField}";
            }
        }

        #endregion
    }
}
