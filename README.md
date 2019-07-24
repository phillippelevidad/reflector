# Usage

``` csharp
var client = Reflector.Create<Client>()
    .Set("Id", "a65s4d6a5s")
    .Set("Domain", "www.some-client.com")
    .Set("RegisteredAt", DateTime.UtcNow)
    .Set("platforms", new[] { 1, 2, 3 }) // field
    .GetInstance();

// Or supply your own instance
var another = Reflector.Using(new Client())
    .Set("Id", "a65s4d6a5s")
    .Set("Domain", "www.some-client.com")
    .GetInstance();
```

Key points:
- The class must provide a parameterless constructor (it may be private);
- Property/field names are case-insensitive;
- Property/fields can be private;
- Fields are searched with the following patterns: `field`, `_field`, `mField`, `m_field`

### Installation

Install from NuGet at
https://www.nuget.org/packages/Reflector.Mapper

### Performance notes

The implementation has been optimized with caching. 

The first time a constructor, property setter or field setter are used, Reflector builds and caches the constructor or setter as a delegate, which allows for repeating the operations in a static fashion. This means that subsequent calls are almost as fast as instantiating classes and setting properties/fields by hand (i.e. without a mapping helper).

-----

### Covered test cases

- Can create class with public constructor
- Can create class with private constructor
- Can set public property
- Can set private property
- Can set nullable property
- Can set field
- Can set property regardless of case
- Property is preferred over field when names collide
- Non-existent property or field returns InvalidOperationException
- Static property or field returns InvalidOperationException
- Readonly property or field returns InvalidOperationException