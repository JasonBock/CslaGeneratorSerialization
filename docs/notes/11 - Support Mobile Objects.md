DONE - For `CslaClaimsPrincipal`:
* DONE - In `ClaimsPrincipalBuilder`
    * DONE - `BuildWriter()` - no need to wrap with `CslaClaimsPrincipal`
    * DONE - `BuildReader()` - same, no wrapper needed.
* DONE - Fields
    * DONE - `_isNew` => `public bool IsNew { get; private set; }`
    * DONE - `_isDeleted` => `public bool IsDeleted { get; private set; }`
    * DONE - `_isDirty` - no change
    * DONE - `_isChild` - no change

Could use a `ITypeSymbol` resolved by `GetTypeByMetadataName`, or do a match on the full name:

```c#
!_.GetAttributes().Any(
	a => a.AttributeClass!.Equals(obsoleteAttribute, SymbolEqualityComparer.Default)
```

TODO:
* `CslaClaimsPrincipal` has disappeared. Huh?!
* I am ignoring `CA1515`, but...can NUnit tests run if they're `internal`?

Future:
* Check to see that the target type already implements `IGeneratorSerializable`. It really shouldn't, but what if it does?
* Look at `UnsafeAccessor` to (potentially) speed up private field access - https://devblogs.microsoft.com/dotnet/performance-improvements-in-net-9/#reflection