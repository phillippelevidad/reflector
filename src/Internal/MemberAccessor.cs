using System;
using System.Reflection;

namespace Internal
{
    internal static class MemberAccessor
    {
        private static readonly BindingFlags BindingFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase;

        internal static MemberInfo GetMemberForReadingOrNull(Type type, string name)
        {
            var property = type.GetProperty(name, BindingFlags);
            if (property != null && property.CanRead) return property;

            foreach (var variation in GetFieldNameVariations(name))
            {
                var field = type.GetField(variation, BindingFlags);
                if (field != null) return field;
            }

            return null;
        }

        internal static MemberInfo GetMemberForWritingOrNull(Type type, string name)
        {
            var property = type.GetProperty(name, BindingFlags);
            if (property != null && property.CanWrite) return property;

            foreach (var variation in GetFieldNameVariations(name))
            {
                var field = type.GetField(variation, BindingFlags);
                if (field != null && !field.IsInitOnly) return field;
            }

            return null;
        }

        internal static string[] GetFieldNameVariations(string name)
        {
            return new string[] { name, $"_{name}", $"m{name}", $"m_{name}" };
        }
    }
}
