namespace tests
{
    public class ClassWithPrivateEmptyConstructor
    {
        private ClassWithPrivateEmptyConstructor()
        {
        }

#pragma warning disable IDE0044 // Add readonly modifier
#pragma warning disable IDE0051 // Remove unused private members
        private string fieldAndPropertyWithSameName;
#pragma warning restore IDE0051 // Remove unused private members
#pragma warning restore IDE0044 // Add readonly modifier

        public string FieldAndPropertyWithSameName { get; set; }
    }
}
