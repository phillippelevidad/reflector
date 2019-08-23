using Internal;
using System.Text;

namespace System.Reflection
{
    public class PropertyOrFieldNotFoundException : Exception
    {
        public PropertyOrFieldNotFoundException(string message) : base(message)
        {
        }

        internal static PropertyOrFieldNotFoundException For(Type type, string name)
        {
            var message = new StringBuilder($"Property or field '{name}' was not found in type '{type.FullName}'.")
                .AppendLine("The following patterns have been searched (search is not case sensitive and looks for writable instance members, whether public or not):")
                .AppendLine($"Property named '{name}'");

            foreach (var variation in MemberAccessor.GetFieldNameVariations(name))
                message.AppendLine($"Field named '{variation}'");

            return new PropertyOrFieldNotFoundException(message.ToString());
        }
    }
}
