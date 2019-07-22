# Usage

Copy and paste the [Reflector](https://github.com/phillippelevidad/reflector/src/Reflector.cs) class directly to your project.

``` csharp
var client = Reflector.Create<Client>()
    .Set("Id", "a65s4d6a5s")
    .Set("Domain", "www.some-client.com")
    .Set("RegisteredAt", DateTime.UtcNow)
    .Set("platforms", new[] { 1, 2, 3 }, isField: true)
    .GetInstance();
```

Key points:
- The class must provide a parameterless constructor (it may be private);
- Property/field names are case-insensitive;
- Property/fields can be private;
- `Set(name, value)` will look for a property, while `Set(name, value, isField: true)` will look for a field.

---

The implementation has been optimized with caching. 

The first time a constructor, property setter or field setter are used, Reflector builds and caches a delegate that allows for repeating the operations in a static fashion. This means that subsequent calls are almost as fast as writing code to instantiate the classes and set properties/fields by hand (i.e. without a mapping helper).