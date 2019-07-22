# Usage

``` csharp
var client = Reflector.Create<Client>()
    .Set("Id", "a65s4d6a5s")
    .Set("Domain", "www.some-client.com")
    .Set("RegisteredAt", DateTime.UtcNow)
    .SetField("platforms", new[] { 1, 2, 3 })
    .GetInstance();
```

Key points:
- The class must provide a parameterless constructor (it may be private);
- Property/field names are case-insensitive;
- Property/fields can be private;

### Installation

1. Copy and paste the [Reflector](https://github.com/phillippelevidad/reflector/blob/master/src/Reflector.cs) class directly to your project;
2. Install NuGet package [System.Reflection.Emit.Lightweight](https://www.nuget.org/packages/System.Reflection.Emit.Lightweight).

### Performance notes

The implementation has been optimized with caching. 

The first time a constructor, property setter or field setter are used, Reflector builds and caches a delegate that allows for repeating the operations in a static fashion. This means that subsequent calls are almost as fast as writing code to instantiate the classes and set properties/fields by hand (i.e. without a mapping helper).