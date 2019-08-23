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

        internal static Setter BuildFor(Type type, string memberName)
        {
            var member = MemberAccessor.GetMemberForWritingOrNull(type, memberName);

            if (member == null)
                throw PropertyOrFieldNotFoundException.For(type, memberName);

            var memberType = member.MemberType == MemberTypes.Property
                ? (member as PropertyInfo).PropertyType
                : (member as FieldInfo).FieldType;

            var setter = BuildForInternal(type, member, memberType);
            var key = $"Set_{type.AssemblyQualifiedName}_{member.Name}";

            return new Setter(key, setter);
        }

        private static Action<object, object> BuildForInternal(Type type, MemberInfo member, Type memberType)
        {
            var target = Expression.Parameter(type, "target");
            var withValue = Expression.Parameter(typeof(object), "value");

            var accessMember = Expression.PropertyOrField(target, member.Name);
            var assign = Expression.Assign(accessMember, Expression.Convert(withValue, memberType));

            return Expression.Lambda<Action<object, object>>(assign, target, withValue).Compile();
        }

        public override string ToString() => key;
    }
}
