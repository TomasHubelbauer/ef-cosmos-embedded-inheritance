# EF Cosmos Embedded Inheritance

In this repository I will attempt to demonstrate an issue I have run into with the EF Core Cosmos DB provider.

This exploration started with me exploring my options for having a navigation property which corresponds to a TS union type.

Let's image we have a TS union type like this:

```typescript
type Lookup =
  | { rule: 'by-first-name'; firstName: string; }
  | { rule: 'by-last-name'; lastName: string; }
  | { rule: 'by-employment-in-date-and-time-range'; from: Date; to: Date; }
  ;
```

This type allows one to store a rule which gets restored and executed on demand to produce search results.
We want to store these queries in a structured way because we only allow some queries not a fully general query language.

For simplicity let's say each admin can have a single favorite filter stored with their profile:

```type
type Admin = {
  firstName: string;
  lastName: string;
  favoriteLookup: Lookup;
};
```

When using EF Core at the BE with the Cosmos provider, to store this, we might model our entities like this:

```csharp
public abstract class Lookup {
  public Guid Id { get; set; }
}

public sealed class LookupByFirstName {
  public string FirstName { get; set; }
}

public sealed class LookupByLastName {
  public string LastName { get; set; }
}

public sealed class LookupByEmploymentInDateAndTimeRange {
  public DateTime From { get; set; }
  public DateTime To { get; set; }
}

public sealed class Admin = {
  public string FirstName { get; set; }
  public string LastName { get; set; }
  public Lookup FavoriteLookup { get; set; }
};
```

This by itself should work. We will need to add `HasBaseType` most likely, but then it should work.

Check out the [first demo](demo1) showing this.

Note that OmniSharp is getting confused with multiple projects in different folders without SLN
files so all demos have a corresponding SLN file.

This will not work without a base type - so with the base type of `object`.
EF Core explicitly doesn't support navigation properties with `object` or `dynamic` types.

You can see the [second demo](demo2) where this is attempted and throws.

By default EF Core will never embed anything unless configured by `OwnsOne` or `OwnsMany`.

- [ ] Demonstrate `OwnsOne` and `OwnsMany` with a non-inheriting entity

However, `HasBaseType` is only available to the `Entity` top-level method in the model builder.
It is not available further in the fluent API chain, so we will not be able to express that a top-level entity
(`Admin`) has a navigation property (`FavoriteLookup`) whose values is a union of possible types with a shared
base (`Lookup`).

This is either a bug or a missing feature or a technical limitation of EF Core.
I want to find out which is it, so I am creating this demo to attach to a GitHub issue I will created in EF Core.

Or so I thought… But the [third demo](demo3) clearly shows it is possible to have an embedded entity which is
at the same time a union type.

This contrasts with my other experiment, [ef-cosmos-union-type](https://github.com/TomasHubelbauer/ef-cosmos-union-type),
where turning on `OwnsOne` causes a model validation exception I also face in the real app I am building.

So there must be something between this simple schema in `demo3` and the more complex one in `ef-cosmos-union-type`
which causes the error and it might be the longer embeddance chain.

## To-Do

### Embed `Admin` into something else to prolong the embedance chain to see if that causes an error

### Try to use `OwnsMany` and introduce multiple favorites
