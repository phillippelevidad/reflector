namespace System.Reflection
{
    public class Reflector
    {
        public static Reflector<T> Create<T>() where T : class
        {
            return Reflector<T>.Create();
        }

        public static Reflector<T> Using<T>(T instance) where T : class
        {
            return Reflector<T>.Using(instance);
        }
    }

    public class Reflector<T> where T : class
    {
        private readonly Type type;
        private readonly T instance;

        private Reflector()
        {
            type = typeof(T);
            instance = CachedActivator.CreateInstance<T>();
        }

        private Reflector(T instance)
        {
            type = typeof(T);
            this.instance = instance;
        }

        public T GetInstance()
        {
            return instance;
        }

        public Reflector<T> Set(string name, object value)
        {
            CachedSetter.PropertyOrField<T>(type, name).Invoke(instance, value);
            return this;
        }

        public static Reflector<T> Create()
        {
            return new Reflector<T>();
        }

        public static Reflector<T> Using(T instance)
        {
            return new Reflector<T>(instance);
        }
    }
}