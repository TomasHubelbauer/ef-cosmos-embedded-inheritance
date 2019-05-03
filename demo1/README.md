# Demo 1

This demo shows a successful use of `HasBaseType` to track entities in Cosmos
which are all possible values of a navigation property but can have different
types, always sharing a base type though.

1. `dotnet new console`
2. `dotnet add package Microsoft.EntityFrameworkCore.Cosmos`
3. `<LangVersion>latest</LangVersion>` for `async Task Main`
4. Ensure the Cosmos emulator is running
5. Download the Cosmos key from the running instance to avoid versioning it
6. Wire up the model and the DB context as follows:

```
Admin
- Id
- First Name
- Last Name
- FavoriteLookup ---------
                          |
                        Lookup
                        - Id
                          |
        ---------------------------------------------
       |                  |                          |
LookupByFirstName  LookupByLastName  LookupByEmploymentInDateAndTimeRange
- FirstName        - LastName        - From
                                     - To
```

7. Configure `HasBaseType` to avoid

> The corresponding CLR type for entity type 'Lookup' is not instantiable and there is no derived entity type in the model that corresponds to a concrete CLR type

8. Use the *Debug demo1* debugger configuration to run and debug the demo

- [ ] Figure out why `FavoriteLookup` is empty at the TODO and crashes with `Include(a => a.FavoriteLookup)`
