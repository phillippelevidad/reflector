using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Internal
{
    internal class Constructor
    {
        private static readonly BindingFlags constructorFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private readonly CreateInstanceDelegate createInstanceDelegate;

        private Constructor(CreateInstanceDelegate createInstanceDelegate)
        {
            this.createInstanceDelegate = createInstanceDelegate;
        }

        internal object Construct() => createInstanceDelegate.Invoke();
        internal T Construct<T>() => (T)Construct();

        internal static Result<Constructor> BuildFor(Type type)
        {
            var createMethod = new DynamicMethod($"Create_{type.Name}", type, Type.EmptyTypes);
            var defaultConstructor = type.GetConstructor(constructorFlags, null, Type.EmptyTypes, null);

            if (defaultConstructor == null)
                return Result.Fail<Constructor>(BuildErrorMessage(type));

            var ilGenerator = createMethod.GetILGenerator();
            ilGenerator.Emit(OpCodes.Newobj, defaultConstructor);
            ilGenerator.Emit(OpCodes.Ret);

            var @delegate = createMethod.CreateDelegate(typeof(CreateInstanceDelegate))
                as CreateInstanceDelegate;

            return new Constructor(@delegate);
        }

        private static string BuildErrorMessage(Type type)
        {
            return $"A default (parameterless) constructor was not found in type '{type.FullName}'. " +
                "Please provide a default constructor, public or not.";
        }

        private delegate object CreateInstanceDelegate();
    }
}
