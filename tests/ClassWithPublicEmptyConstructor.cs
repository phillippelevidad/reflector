using System;
using System.Diagnostics.CodeAnalysis;

namespace tests
{
    public class ClassWithPublicEmptyConstructor
    {
        [SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification = "Needed for test case")]
        [SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Needed for test case")]
        private bool _someField;

        [SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Needed for test case")]
        private readonly string mReadOnlyField;

        [SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification = "<Pending>")]
        [SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Needed for test case")]
        private static string staticField;

        public int PublicProperty { get; private set; }
        public DateTime? NullableProperty { get; private set; }

        [SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Needed for test case")]
        private string PrivateProperty { get; set; }

        public static string StaticProperty { get; set; }

        public string ReadOnlyProperty { get; }


        public ClassWithPublicEmptyConstructor()
        {
        }
    }
}
