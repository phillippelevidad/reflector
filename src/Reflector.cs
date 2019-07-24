namespace System.Reflection
{
    public class Reflector
    {
        public static Reflector<T> Create<T>() where T : class
        {
            return Reflector<T>.Create();
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
    }
}