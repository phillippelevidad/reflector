using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Text;

namespace System.Reflection
{
    internal class CachedSetter
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
                        expression = PropertyOrFieldInternal<TInstance>(type, name);
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
}
