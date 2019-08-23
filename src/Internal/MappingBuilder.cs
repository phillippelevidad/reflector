using System;
using System.Collections.Generic;
using System.Reflection;

namespace Internal
{
    internal class Mapping
    {
        private readonly Type sourceType;
        private readonly Type targetType;
        private readonly IEnumerable<MemberMapping> memberMappings;

        private Mapping(Type sourceType, Type targetType, IEnumerable<MemberMapping> memberMappings)
        {
            this.sourceType = sourceType;
            this.targetType = targetType;
            this.memberMappings = memberMappings;
        }

        internal void Map(object source, object target)
        {
            foreach (var memberMapping in memberMappings)
            {
                var value = memberMapping.SourceGetter.Get(source);
                memberMapping.TargetSetter.Set(target, value);
            }
        }

        internal static Mapping BuildFor<TSource, TTarget>()
        {
            var sourceType = typeof(TSource);
            var targetType = typeof(TTarget);
            var memberMappings = new List<MemberMapping>();

            foreach (var sourceProperty in sourceType.GetProperties())
            {
                var sourceMember = MemberAccessor.GetMemberForReadingOrNull(sourceType, sourceProperty.Name);
                if (sourceMember == null)
                    continue;

                var targetMember = MemberAccessor.GetMemberForWritingOrNull(targetType, sourceProperty.Name);
                if (targetMember == null)
                    continue;

                var sourceMemberType = GetPropertyOrFieldType(sourceMember);
                var targetMemberType = GetPropertyOrFieldType(targetMember);
                if (sourceMemberType != targetMemberType)
                    continue;

                var sourceGetter = Getter.BuildFor(sourceType, sourceMember.Name);
                var targetSetter = Setter.BuildFor(targetType, sourceMember.Name);

                memberMappings.Add(new MemberMapping(sourceGetter, targetSetter));
            }

            return new Mapping(sourceType, targetType, memberMappings);
        }

        public override string ToString()
        {
            return $"{sourceType.AssemblyQualifiedName}|{targetType.AssemblyQualifiedName}";
        }

        private static Type GetPropertyOrFieldType(MemberInfo member)
        {
            if (member.MemberType == MemberTypes.Property)
                return (member as PropertyInfo).PropertyType;

            if (member.MemberType == MemberTypes.Field)
                return (member as FieldInfo).FieldType;

            throw new InvalidOperationException($"A member of type PropertyInfo or FieldInfo is expected. Found {member.MemberType}, instead.");
        }

        private class MemberMapping
        {
            internal MemberMapping(Getter sourceGetter, Setter targetSetter)
            {
                SourceGetter = sourceGetter;
                TargetSetter = targetSetter;
            }

            internal Getter SourceGetter { get; }
            internal Setter TargetSetter { get; }
        }
    }
}
