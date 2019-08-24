using FluentAssertions;
using System;
using System.Reflection;
using Xunit;

namespace tests
{
    public class ReflectorMapperAssertions
    {
        [Fact]
        public void CanMapExactlyMatchingTypes()
        {
            var source = new Source();
            var target = ReflectorMapper.Map<Target>(source);

            target.Id.Should().Be(source.Id);
            target.Name.Should().Be(source.Name);
            target.CreatedAt.Should().Be(source.CreatedAt);
        }

        [Fact]
        public void CanMapToBackingFields()
        {
            var source = new Source();
            var target = ReflectorMapper.Map<TargetWithBackingFields>(source);

            target.Id.Should().Be(source.Id);
            target.Name.Should().Be(source.Name);
            target.CreatedAt.Should().Be(source.CreatedAt);
        }
    }

    public class Source
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = "John Doe";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    public class Target
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public DateTime CreatedAt { get; private set; }
    }

    public class TargetWithBackingFields
    {
        private Guid id;
        private string _name;
        private DateTime m_createdAt;

        public Guid Id => id;
        public string Name => _name;
        public DateTime CreatedAt => m_createdAt;
    }
}
