using System;

namespace tests
{
    public class ClassWithPublicEmptyConstructor
    {
        private bool _someField;
        private readonly string mReadOnlyField;
        private static string staticField;

        public int PublicProperty { get; private set; }
        public DateTime? NullableProperty { get; private set; }
        private string PrivateProperty { get; set; }

        public static string StaticProperty { get; set; }

        public string ReadOnlyProperty { get; }


        public ClassWithPublicEmptyConstructor()
        {
        }
    }
}
