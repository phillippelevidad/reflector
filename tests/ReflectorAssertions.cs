using FluentAssertions;
using System;
using System.Reflection;
using Xunit;

namespace tests
{
    public class ReflectorAssertions
    {
        [Fact]
        public void CanCreateClassWithPublicEmptyConstructor()
        {
            var instance = Reflector.Create<ClassWithPublicEmptyConstructor>().GetInstance();
            instance.Should().BeOfType<ClassWithPublicEmptyConstructor>();
        }

        [Fact]
        public void CanCreateClassWithPrivateEmptyConstructor()
        {
            var instance = Reflector.Create<ClassWithPrivateEmptyConstructor>().GetInstance();
            instance.Should().BeOfType<ClassWithPrivateEmptyConstructor>();
        }

        [Fact]
        public void CanSetPublicProperty()
        {
            var instance = Reflector.Create<ClassWithPublicEmptyConstructor>()
                .Set("PublicProperty", 10)
                .GetInstance();

            instance.PublicProperty.Should().Be(10);
        }

        [Fact]
        public void CanSetPrivateProperty()
        {
            var instance = Reflector.Create<ClassWithPublicEmptyConstructor>()
                .Set("PrivateProperty", "some value")
                .GetInstance();

            var prop = typeof(ClassWithPublicEmptyConstructor).GetProperty("PrivateProperty", BindingFlags.Instance | BindingFlags.NonPublic);
            var value = prop.GetValue(instance, null);

            value.Should().Be("some value");
        }

        [Fact]
        public void CanSetNullablePropertyWithAValue()
        {
            var value = DateTime.Now;
            var instance = Reflector.Create<ClassWithPublicEmptyConstructor>()
                .Set("NullableProperty", value)
                .GetInstance();

            instance.NullableProperty.Should().Be(value);
        }

        [Fact]
        public void CanSetNullablePropertyWithNull()
        {
            var instance = Reflector.Create<ClassWithPublicEmptyConstructor>()
                .Set("NullableProperty", null)
                .GetInstance();

            instance.NullableProperty.Should().BeNull();
        }

        [Fact]
        public void CanSetField()
        {
            var instance = Reflector.Create<ClassWithPublicEmptyConstructor>()
                .Set("someField", true)
                .GetInstance();

            var field = typeof(ClassWithPublicEmptyConstructor).GetField("_someField", BindingFlags.Instance | BindingFlags.NonPublic);
            var value = field.GetValue(instance);

            value.Should().Be(true);
        }

        [Fact]
        public void CanSetAllFieldsAndPropertiesInOneGo()
        {
            var now = DateTime.Now;
            var instance = Reflector.Create<ClassWithPublicEmptyConstructor>()
                .Set("PublicProperty", 10)
                .Set("PrivateProperty", "some value")
                .Set("NullableProperty", now)
                .Set("someField", true)
                .GetInstance();

            var prop = typeof(ClassWithPublicEmptyConstructor).GetProperty("PrivateProperty", BindingFlags.Instance | BindingFlags.NonPublic);
            var propValue = prop.GetValue(instance, null);

            var field = typeof(ClassWithPublicEmptyConstructor).GetField("_someField", BindingFlags.Instance | BindingFlags.NonPublic);
            var fieldValue = field.GetValue(instance);

            propValue.Should().Be("some value");
            fieldValue.Should().Be(true);
            instance.PublicProperty.Should().Be(10);
            instance.NullableProperty.Should().Be(now);
        }

        [Fact]
        public void CanSetPropertyRegardlessOfCase()
        {
            var instance = Reflector.Create<ClassWithPrivateEmptyConstructor>()
                .Set("FiElDaNdPrOpErTyWiThSaMeNaMe", "case doesn't matter")
                .GetInstance();

            instance.FieldAndPropertyWithSameName.Should().Be("case doesn't matter");
        }

        [Fact]
        public void PropertyIsPreferredWhenNameCollidesWithField()
        {
            var instance = Reflector.Create<ClassWithPrivateEmptyConstructor>()
                .Set("FieldAndPropertyWithSameName", "value")
                .GetInstance();

            var field = typeof(ClassWithPrivateEmptyConstructor).GetField("fieldAndPropertyWithSameName", BindingFlags.Instance | BindingFlags.NonPublic);
            var fieldValue = field.GetValue(instance);

            instance.FieldAndPropertyWithSameName.Should().Be("value");
            fieldValue.Should().BeNull();
        }

        [Fact]
        public void NonExistingPropertyOrFieldThrowsInvalidOperationException()
        {
            var reflector = Reflector.Create<ClassWithPublicEmptyConstructor>();
            Action invalidAction = () => reflector.Set("NonExistingPropertyOrField", "value");
            invalidAction.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void StaticPropertyOrFieldThrowsInvalidOperationException()
        {
            var reflector = Reflector.Create<ClassWithPublicEmptyConstructor>();

            Action invalidProperty = () => reflector.Set("StaticProperty", "value");
            Action invalidField = () => reflector.Set("staticField", "value");

            invalidProperty.Should().Throw<InvalidOperationException>();
            invalidField.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void ReadOnlyPropertyOrFieldThrowsInvalidOperationException()
        {
            var reflector = Reflector.Create<ClassWithPublicEmptyConstructor>();

            Action invalidProperty = () => reflector.Set("ReadOnlyProperty", "value");
            Action invalidField = () => reflector.Set("readOnlyField", "value");

            invalidProperty.Should().Throw<InvalidOperationException>();
            invalidField.Should().Throw<InvalidOperationException>();
        }
    }
}
