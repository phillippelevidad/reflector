using Internal;

namespace System.Reflection
{
    public class Reflector
    {
        private readonly Type type;
        private readonly object instance;

        private Reflector(Type type, object instance)
        {
            this.type = type;
            this.instance = instance;
        }

        public static Reflector Create(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            var instance = ExpressionCache.GetConstructor(type).Construct();
            return new Reflector(type, instance);
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
            return new Reflector(type, instance);
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
            ExpressionCache.GetSetter(type, memberName).Set(instance, value);
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