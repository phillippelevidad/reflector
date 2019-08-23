using Internal;

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
        private readonly T instance;

        private Reflector()
        {
            instance = ExpressionCache.GetConstructor<T>().Construct<T>();
        }

        private Reflector(T instance)
        {
            this.instance = instance;
        }

        public T GetInstance()
        {
            return instance;
        }

        public Reflector<T> Set(string memberName, object value)
        {
            ExpressionCache.GetSetter<T>(memberName).Set(instance, value);
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