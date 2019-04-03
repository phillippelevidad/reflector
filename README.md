# Usage

``` csharp
var client = Reflector.Create<Client>()
    .Set("Id", "a65s4d6a5s")
    .Set("Domain", "www.some-client.com")
    .Set("RegisteredAt", DateTime.UtcNow)
    .Set("platforms", new[] { 1, 2, 3 }, isField: true)
    .GetInstance();
```

- Property/field names are case-insensitive;
- Property/fields can be private;
- `Set(name, value)` will look for a property, while `Set(name, value, isField: true)` will look for a field.
