using Internal;

namespace System.Reflection
{
    public class Reflector
    {
        private readonly object instance;
        private readonly TypeConstructorAndSetters constructorAndSetters;

        private Reflector(object instance, TypeConstructorAndSetters constructorAndSetters)
        {
            this.instance = instance;
            this.constructorAndSetters = constructorAndSetters;
        }

        public static Reflector Create(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            var constructorAndSetters = Cache.GetTypeConstructorAndSetters(type);

            if (constructorAndSetters.ConstructorOrNull == null)
                throw new InvalidOperationException($"Did not found a default constructor for type {type.AssemblyQualifiedName}.");

            var instance = constructorAndSetters.ConstructorOrNull.Construct();
            return new Reflector(instance, constructorAndSetters);
        }

        public static Reflector<T> Create<T>() where T : class
        {
            var wrappedReflector = Create(typeof(T));
            return new Reflector<T>(wrappedReflector);
        }

        public static Reflector Using(object instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            var type = instance.GetType();
            var constructorAndSetters = Cache.GetTypeConstructorAndSetters(type);
            return new Reflector(instance, constructorAndSetters);
        }

        public static Reflector<T> Using<T>(T instance) where T : class
        {
            var wrappedReflector = Using((object)instance);
            return new Reflector<T>(wrappedReflector);
        }

        public object GetInstance()
        {
            return instance;
        }

        public Reflector Set(string memberName, object value)
        {
            var setterOrNull = constructorAndSetters.GetSetterOrNull(memberName);

            if (setterOrNull == null)
                throw new InvalidOperationException($"Could not set property of field '{memberName}'.");

            setterOrNull.Set(instance, value);
            return this;
        }
    }

    public class Reflector<T> where T : class
    {
        private readonly Reflector wrappedReflector;

        internal Reflector(Reflector wrappedReflector)
        {
            this.wrappedReflector = wrappedReflector;
        }

        public T GetInstance()
        {
            return (T)wrappedReflector.GetInstance();
        }

        public Reflector<T> Set(string memberName, object value)
        {
            wrappedReflector.Set(memberName, value);
            return this;
        }
    }
}