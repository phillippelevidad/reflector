using System;

namespace tests
{
    public class ClassWithPublicEmptyConstructor
    {
        private bool someField;
        public int PublicProperty { get; set; }
        public DateTime? NullableProperty { get; set; }
        private string PrivateProperty { get; set; }

        public ClassWithPublicEmptyConstructor()
        {
        }
    }
}
