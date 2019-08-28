using System;
using System.Collections.Generic;

namespace Internal
{
    internal class TypeConstructorAndSetters
    {
        private readonly List<KeyValuePair<string, Setter>> membersWithSetters;

        public Constructor ConstructorOrNull { get; }

        private TypeConstructorAndSetters(Constructor constructorOrNull, List<KeyValuePair<string, Setter>> membersWithSetters)
        {
            ConstructorOrNull = constructorOrNull;
            this.membersWithSetters = membersWithSetters;
        }

        public Setter GetSetterOrNull(string memberName)
        {
            foreach (var setter in membersWithSetters)
            {
                if (setter.Key.Equals(memberName, StringComparison.OrdinalIgnoreCase))
                    return setter.Value;

                var nameVariations = GetMemberNameVariations(memberName);

                foreach (var name in nameVariations)
                    if (setter.Key.Equals(name, StringComparison.OrdinalIgnoreCase))
                        return setter.Value;
            }

            return null;
        }

        internal string[] GetMemberNameVariations(string name)
        {
            return new string[] { name, $"_{name}", $"m{name}", $"m_{name}" };
        }

        internal static TypeConstructorAndSetters BuildFor(Type type)
        {
            var constructor = Constructor.BuildFor(type);
            var members = MemberAccessor.ListMembersForWriting(type);
            var membersWithSetters = new List<KeyValuePair<string, Setter>>(members.Count);

            foreach (var member in members)
            {
                var setter = Setter.BuildFor(type, member.Name);

                if (setter.IsFailure)
                    continue;

                membersWithSetters.Add(new KeyValuePair<string, Setter>(member.Name, setter.Value));
            }

            return new TypeConstructorAndSetters(
                constructor.IsSuccess ? constructor.Value : null,
                membersWithSetters);
        }
    }
}
