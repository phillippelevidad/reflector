using System;
using System.Diagnostics;
using System.Reflection;

namespace benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            var loop = 10_000_000;

            var reflector = Reflector.Create<MyClass>();
            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < loop; i++)
            {
                reflector.Set("MyProperty", i);
            }
            stopwatch.Stop();
            Console.WriteLine("Reflector: " + stopwatch.ElapsedMilliseconds);

            var instance = reflector.GetInstance();
            stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < loop; i++)
            {
                instance.MyProperty = i;
            }
            stopwatch.Stop();
            Console.WriteLine("Direct: " + stopwatch.ElapsedMilliseconds);

            stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < loop; i++)
            {
                var property = instance.GetType().GetProperty("MyProperty");
                property.SetValue(instance, i, null);
            }
            stopwatch.Stop();
            Console.WriteLine("System.Reflection: " + stopwatch.ElapsedMilliseconds);
        }
    }

    public class MyClass
    {
        public int MyProperty { get; set; }
    }
}
