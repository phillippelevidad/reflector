using System;

namespace System.Reflection
{
    public class Reflector
    {
        public static Reflector<T> Create<T>() where T : class
        {
            return Reflector<T>.CreateInstance<T>();
        }
    }

    public class Reflector<T> where T : class
    {
        private readonly BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase;
        private readonly Type type;
        private readonly T instance;

        private Reflector()
        {
            type = typeof(T);
            instance = Activator.CreateInstance(type, nonPublic: true) as T;
        }

        public T GetInstance()
        {
            return instance;
        }

        public Reflector<T> Set(string name, object value, bool isField = false)
        {
            return isField ? SetField(name, value) : SetProperty(name, value);
        }

        public Reflector<T> SetField(string name, object value)
        {
            type.GetField(name, flags).SetValue(instance, value);
            return this;
        }

        public Reflector<T> SetProperty(string name, object value)
        {
            type.GetProperty(name, flags).SetValue(instance, value);
            return this;
        }

        public static Reflector<TModel> CreateInstance<TModel>() where TModel : class
        {
            return new Reflector<TModel>();
        }
    }
}
