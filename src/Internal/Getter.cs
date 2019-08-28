using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Internal
{
    internal class Getter
    {
        private readonly string key;
        private readonly Func<object, object> getter;

        private Getter(string key, Func<object, object> getter)
        {
            this.key = key;
            this.getter = getter;
        }

        internal object Get(object fromInstance) => getter.Invoke(fromInstance);

        internal static Result<Getter> BuildFor(Type type, string memberName)
        {
            var memberResult = MemberAccessor.GetMemberForReading(type, memberName);
            if (memberResult.IsFailure)
                return Result.Fail<Getter>(memberResult.Error);

            var member = memberResult.Value;
            var fromSource = Expression.Parameter(typeof(object), "source");
            var accessMember = member.MemberType == MemberTypes.Property
                ? Expression.Property(Expression.Convert(fromSource, type), member.Name)
                : Expression.Field(Expression.Convert(fromSource, type), member.Name);
            var returnValue = Expression.Convert(accessMember, typeof(object));

            var getter = Expression.Lambda<Func<object, object>>(returnValue, fromSource).Compile();
            var key = $"Get_{type.AssemblyQualifiedName}_{member.Name}";

            return new Getter(key, getter);
        }

        public override string ToString() => key;
    }
}
