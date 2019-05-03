# Demo 2

This is the same demo as demo #1 but it uses `object` as a common base type,
not `Lookup`. I am interested in finding out if this will result in correct
typing of the `FavoriteLookup` navigation property still.

See the README of the [first demo](../demo1) for more information.

Turns out `object` cannot be used as a type for `FavoriteLookup` because EF Core
explicitly doesn't support it and will throw an exception.
Same goes for `dynamic` which EF Core sees as `object` as well.

So it wouldn't be possible to use `object` as the type and then rely on EF Core
to map it to the correct type based on the `Discriminator` so that we could then
`switch` on the `object` to get branches with correctly typed entity based on
the discriminator-typed EF Core result placed into the `object`-typed property.
