namespace tests
{
    public class ClassWithPrivateEmptyConstructor
    {
        private ClassWithPrivateEmptyConstructor()
        {
        }

        private string fieldAndPropertyWithSameName;
        public string FieldAndPropertyWithSameName { get; set; }
    }
}
