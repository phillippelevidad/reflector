using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Internal
{
    internal class Setter
    {
        private readonly string key;
        private readonly Action<object, object> setter;

        private Setter(string key, Action<object, object> setter)
        {
            this.key = key;
            this.setter = setter;
        }

        internal void Set(object instance, object value) => setter.Invoke(instance, value);

        internal static Result<Setter> BuildFor(Type type, string memberName)
        {
            var memberResult = MemberAccessor.GetMemberForWritingOrNull(type, memberName);
            if (memberResult.IsFailure)
                return Result.Fail<Setter>(memberResult.Error);

            var member = memberResult.Value;
            var memberType = member.MemberType == MemberTypes.Property
                ? (member as PropertyInfo).PropertyType
                : (member as FieldInfo).FieldType;

            var action = BuildForInternal(type, member, memberType);
            var key = $"Set_{type.AssemblyQualifiedName}_{member.Name}";

            return new Setter(key, action);
        }

        private static Action<object, object> BuildForInternal(Type type, MemberInfo member, Type memberType)
        {
            var target = Expression.Parameter(typeof(object), "target");
            var withValue = Expression.Parameter(typeof(object), "value");

            var accessMember = Expression.PropertyOrField(Expression.Convert(target, type), member.Name);
            var assign = Expression.Assign(accessMember, Expression.Convert(withValue, memberType));

            return Expression.Lambda<Action<object, object>>(assign, target, withValue).Compile();
        }

        public override string ToString() => key;
    }
}
