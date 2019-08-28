using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Internal
{
    internal static class MemberAccessor
    {
        private static readonly BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase;

        internal static Result<MemberInfo> GetMemberForReading(Type type, string name)
        {
            var property = type.GetProperty(name, bindingFlags);
            if (property != null && property.CanRead) return property as MemberInfo;

            foreach (var variation in GetFieldNameVariations(name))
            {
                var field = type.GetField(variation, bindingFlags);
                if (field != null) return field as MemberInfo;
            }

            return null;
        }

        internal static Result<MemberInfo> GetMemberForWriting(Type type, string name)
        {
            var property = type.GetProperty(name, bindingFlags);
            if (property != null && property.CanWrite)
                return property as MemberInfo;

            foreach (var variation in GetFieldNameVariations(name))
            {
                var field = type.GetField(variation, bindingFlags);
                if (field != null && !field.IsInitOnly) return field as MemberInfo;
            }

            var message = BuildErrorMessage(type, name);
            return Result.Fail<MemberInfo>(message);
        }

        internal static IReadOnlyCollection<MemberInfo> ListMembersForWriting(Type type)
        {
            return type.GetProperties(bindingFlags)
                .Where(property => property.CanWrite)
                .Select(property => property as MemberInfo)
                .Union(type.GetFields(bindingFlags)
                        .Where(field => !field.IsInitOnly)
                        .Select(field => field as MemberInfo))
                .ToList();
        }

        internal static string[] GetFieldNameVariations(string name)
        {
            return new string[] { name, $"_{name}", $"m{name}", $"m_{name}" };
        }

        private static string BuildErrorMessage(Type type, string name)
        {
            var message = new StringBuilder($"Property or field '{name}' was not found in type '{type.FullName}'.")
                .AppendLine("The following patterns have been searched (search is not case sensitive and looks for writable instance members, whether public or not):")
                .AppendLine($"Property named '{name}'");

            foreach (var variation in GetFieldNameVariations(name))
                message.AppendLine($"Field named '{variation}'");

            return message.ToString();
        }
    }
}
