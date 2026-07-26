I've got problems:
* `GetUnionCaseTypes()` won't stop if there are recursive unions
* On the reader side, I need to know what the type is, so I'll have to store a count/ID value before the actual value
* Even with that, I'll also need to construct the type if it's another union type
* I **may** have an "out" - https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/preview6/libraries.md#systemtextjson-serializes-c-union-types

TODO:
* Need a test for `GetFullyQualifiedName()` in `StringExtensions`
* Why does `EquatableArray<>` not like it when you assign `[]` to a value and then look at `.IsEmpty` or `.Length`?