# Reflector.cs Usage

```
var client = Reflector.Create<Client>()
    .Set("Id", "a65s4d6a5s")
    .Set("Domain", "www.some-client.com")
    .Set("RegisteredAt", DateTime.UtcNow)
    .Set("Platforms", new[] { 1, 2, 3 }, isField: true)
    .GetInstance();
```

Reflector can deal with `private set` properties and `private` fields as well (just remember to indicate that with the `isField: true` parameter when calling the `Set(...)` method.
