using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Text;

namespace System.Reflection
{
    // Important: you must install package System.Reflection.Emit.Lightweight
    // https://www.nuget.org/packages/System.Reflection.Emit.Lightweight

    public class Reflector
    {
        public static Reflector<T> Create<T>() where T : class
        {
            return Reflector<T>.Create<T>();
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

        public Reflector<T> Set(string name, object value)
        {
            CachedSetter.PropertyOrField<T>(type, name).Invoke(instance, value);
            return this;
        }

        public static Reflector<TModel> Create<TModel>() where TModel : class
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

            internal static Action<TInstance, object> PropertyOrField<TInstance>(Type type, string name)
            {
                var key = GetKey(type, name);

                if (!expressionCache.TryGetValue(key, out Delegate expression))
                {
                    lock (type)
                    {
                        if (!expressionCache.TryGetValue(key, out expression))
                        {
                            expression = PropertyOrFieldInternal<T>(type, name);
                            expressionCache[key] = expression;
                        }
                    }
                }

                return (Action<TInstance, object>)expression;
            }

            private static Action<TInstance, object> PropertyOrFieldInternal<TInstance>(Type type, string name)
            {
                var property = type.GetProperty(name, flags);
                if (property != null && property.CanWrite) return GenerateSetter<TInstance>(type, property, property.PropertyType);

                foreach (var variation in GetFieldNameVariations(name))
                {
                    var field = type.GetField(variation, flags);
                    if (field != null && !field.IsInitOnly) return GenerateSetter<TInstance>(type, field, field.FieldType);
                }

                throw GenerateNotFoundException(type, name);
            }

            private static Exception GenerateNotFoundException(Type type, string name)
            {
                var message = new StringBuilder($"Property or field '{name}' was not found in type '{type.FullName}'.")
                    .AppendLine("The following patterns have been searched (search is not case sensitive and looks for writable instance members, whether public or not):")
                    .AppendLine($"Property named '{name}'");

                foreach (var variation in GetFieldNameVariations(name))
                    message.AppendLine($"Field named '{variation}'");

                return new InvalidOperationException(message.ToString());
            }

            private static Action<TInstance, object> GenerateSetter<TInstance>(Type type, MemberInfo member, Type memberType)
            {
                var target = Expression.Parameter(type, "target");
                var withValue = Expression.Parameter(typeof(object), "value");

                var accessMember = Expression.PropertyOrField(target, member.Name);
                var assign = Expression.Assign(accessMember, Expression.Convert(withValue, memberType));

                return Expression.Lambda<Action<TInstance, object>>(assign, target, withValue).Compile();
            }

            private static string[] GetFieldNameVariations(string name)
            {
                return new string[] { name, $"_{name}", $"m{name}", $"m_{name}" };
            }

            private static string GetKey(Type type, string propertyOrField)
            {
                return $"{type.FullName}_{propertyOrField}";
            }
        }

        #endregion
    }
}
