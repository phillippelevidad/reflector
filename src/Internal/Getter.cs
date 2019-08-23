using System;
using System.Linq.Expressions;

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
            var memberResult = MemberAccessor.GetMemberForWritingOrNull(type, memberName);
            if (memberResult.IsFailure)
                return Result.Fail<Getter>(memberResult.Error);

            var member = memberResult.Value;
            var fromSource = Expression.Parameter(type, "source");
            var accessMember = Expression.PropertyOrField(fromSource, member.Name);

            var getter = Expression.Lambda<Func<object, object>>(accessMember, fromSource).Compile();
            var key = $"Get_{type.AssemblyQualifiedName}_{member.Name}";

            return new Getter(key, getter);
        }

        public override string ToString() => key;
    }
}
