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

        internal static Mapping BuildFor(Type sourceType, Type targetType)
        {
            var memberMappings = new List<MemberMapping>();

            foreach (var sourceProperty in sourceType.GetProperties())
            {
                var sourceMember = MemberAccessor.GetMemberForReading(sourceType, sourceProperty.Name);
                if (sourceMember.IsFailure)
                    continue;

                var targetMember = MemberAccessor.GetMemberForWriting(targetType, sourceProperty.Name);
                if (targetMember.IsFailure)
                    continue;

                var sourceMemberType = GetPropertyOrFieldType(sourceMember.Value);
                var targetMemberType = GetPropertyOrFieldType(targetMember.Value);
                if (sourceMemberType != targetMemberType)
                    continue;

                var sourceGetter = Getter.BuildFor(sourceType, sourceMember.Value.Name);
                var targetSetter = Setter.BuildFor(targetType, targetMember.Value.Name);

                memberMappings.Add(new MemberMapping(sourceGetter.Value, targetSetter.Value));
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
